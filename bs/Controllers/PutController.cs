using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using bs;
using bs.Data;

namespace bs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PutController : ControllerBase
    {
        private readonly ILogger<PutController> Logger;
        private IConfiguration Cfg;
        private string[]? sp = null;
        private PutController() { }
        public PutController(IConfiguration cfg, ILogger<PutController> _logger, IHostEnvironment env)
        {
            Cfg = cfg;
            Logger = _logger;
            var fname = System.IO.Path.Combine(env.ContentRootPath, "exclude.txt");
            if (System.IO.File.Exists(fname))
            {
                var lst = new List<string>();
                sp = System.IO.File.ReadLines(fname)
                    .Where(s => !String.IsNullOrWhiteSpace(s) && !s.StartsWith("//") && s.Trim().Length == 31)
                    .ToArray();
            }
        }
        [HttpPut]
        //[Authorize(Policy = "Put")]
        [AllowAnonymous]
        public async Task<ApiResponse> Get()
        {
            return await CreatePuttingDocument();
        }

        private async Task<ApiResponse> CreatePuttingDocument()
        {
            var rt = new ApiResponse();
            try
            {
                var os = GetObjects();
                foreach (var o in os)
                {
                    var js = JsonSerializer.Serialize(o, typeof(LP_FTS_INTRODUCE));
                    var so = Encoding.UTF8.GetBytes(js);
                    var bm = Convert.ToBase64String(so);
                    var sign = HSignManaged.HSign.Sign(so, Cfg.GetValue<string>("certThumbprint"));
                    var docCreate = new lk_documents_create_request()
                    {
                        document_format = "MANUAL",
                        product_document = bm,
                        signature = Convert.ToBase64String(sign),
                        type = "LP_FTS_INTRODUCE"
                    };
                    Logger.LogInformation("Аутентификация пройдена.");
                    string tg = Const.dictTg[o.products_list[0].Tg];
                    var resp = await ProgramHttp_TrueApi_v4.Post2TrueApiAsync2<lk_documents_create_request>
                        (docCreate, "lk/documents/create?pg=" + tg, Cfg);
                    Logger.LogInformation(rt.AddMsg($"Кодов маркировки по документу {resp} {o.products_list.Count} шт."));
                    foreach (var km in o.products_list)
                    {
                        Logger.LogInformation($"{km.cis}");
                    }
                    Save(o.products_list, resp);
                    Logger.LogInformation(rt.AddMsg($"ИД документа {resp} сохранен В БД"));
                }
                rt.status = 0;
                return rt;
            }
            catch (SaException e)
            {
                rt.status = 1;
                Logger.LogError(rt.AddMsg(e.Message));
                return rt;
            }
            catch (Exception e)
            {
                rt.status = 2;
                Logger.LogError(rt.AddMsg(e.Message));
                return rt;
            }
        }
        private void Save(List<Product> products_list, string resp)
        {
            using (var cn = new SqlConnection(Cfg.GetConnectionString("cn_put")))
            {
                cn.Open();
                SqlCommand cmd;
                cmd = new SqlCommand("sp_Mark_RUC_Query", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                foreach (var o in products_list)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@type", "update_docid");
                    cmd.Parameters.AddWithValue("@DocUid", new Guid(resp));
                    cmd.Parameters.AddWithValue("@UniqueId", o.cis);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private List<LP_FTS_INTRODUCE> GetObjects()
        {
            LP_FTS_INTRODUCE o = null;
            var lpf = new List<LP_FTS_INTRODUCE>();
            using (var cn = new SqlConnection(Cfg.GetConnectionString("cn_put")))
            {
                cn.Open();
                SqlCommand cmd;
                cmd = new SqlCommand("sp_Mark_RUC_Query", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "select_create_doc");
                var lp = new List<Product>();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string GTDNum = dr.GetString(1);
                        if (!String.IsNullOrWhiteSpace(GTDNum))
                        {
                            string UniqueId = dr.GetString(0);
                            DateTime GTDDate = dr.GetDateTime(2);
                            string tg = dr.GetString(3);
                            var product = new Product { cis = UniqueId, packType = "UNIT", GTDDate = GTDDate, GTDNum = GTDNum, Tg = tg };
                            if (sp == null || !sp.Contains(UniqueId))
                                lp.Add(product);
                        }
                    }
                }
                if (lp.Count > 0)
                {
                    List<List<Product>> lps = SplitLp(lp);
                    foreach (var lp0 in lps)
                    {
                        o = new LP_FTS_INTRODUCE
                        {
                            declaration_date = lp0[0].GTDDate.ToString("yyyy-MM-dd"),
                            declaration_number = lp0[0].GTDNum,
                            products_list = lp0,
                            trade_participant_inn = Cfg.GetValue<string>("inn")
                        };
                        lpf.Add(o);
                    }
                }
                else
                    throw new SaException("Отсутствуют невведенные в оборот коды маркировки.");
            }
            return lpf;
        }
        private static List<List<Product>> SplitLp(List<Product> lp)
        {
            var rt = new List<List<Product>>();
            var tgnum = lp.Select(s => new { s.Tg, s.GTDNum }).Distinct();
            foreach (var o in tgnum)
            {
                rt.Add(lp.Where(s => s.Tg == o.Tg && s.GTDNum == o.GTDNum).ToList());
            }
            return rt;
        }
        //private string[] sp = new string[]
        //{
        //    // (06) отсутствуют в декларации
        //    "010460718936173921GrE_B:Pc4VSpO",
        //    "010460718936173921uuYghsCFhlHek",
        //};
        // заплатка в связи с вводом в оборот на тестовой базе (помечено kvit=1, DocID - неверный)
        //private string[] sp = new string[] {
        //    "010460718936130221F+vGln*h-N%Yq",
        //    "010460718936121021Kny;C/tkYdjhm",
        //    "010460718936122721<t;?TE?QF2bOJ",
        //    "0104607189361227212(CKrAeUe-AJ8",
        //    "010460718936122721ddSEt=N9KB:o:",
        //    "010460718936122721jab2gp;L,&(iq",
        //    "010460718936122721l9rmQf9kFza=M",
        //    "010460718936122721zptOHUWz\"iF-O",
        //    "010460718936124121A(&:NVgQinrT&",
        //    "010460718936124121gu'!F*r-YRO%:",
        //    "010460718936124121peoDaQ.heW_AM",
        //    "010460718936121021r!VPqu66eMqW,",
        //    "010460718936122721%0i9>Xd(K_QOs",
        //    "010460718936122721&MQfsr'juxLFu",
        //    "010460718936122721*!VTdXHQ=ZO,q",
        //    "010460718936122721a&dxKHdKp-W)5",
        //    "010460718936122721BBf%r,n/Pz>fK",
        //    "010460718936122721bP1sqMZ0dFV=A",
        //    "010460718936122721HdET3D&uExMqS",
        //    "010460718936122721l;DhnELJyT<g!",
        //    "010460718936122721LCQ%8QHml6CJa",
        //    "010460718936122721NdMcGPYQ'Hv5S",
        //    "010460718936122721QErh7'kByU*)h",
        //    "010460718936122721xIYBLyLCrYBjM",
        //    "010460718936124121;?ttpBN&N(O;W",
        //    "01046071893612412155&gn+uSTUL)x",
        //    "0104607189361241217NVNlM?\"cNZef",
        //    "010460718936124121bhtEstT<VpUeq",
        //    "010460718936124121f1a-Ef?gXds6p",
        //    "010460718936124121qEQCthT'ack>G",
        //    "010460718936124121TUlL!.J!U,Nto",
        //    "010460718936130221*uJ>(HJ8kn.J=",
        //    "010460718936130221LE'hsXqMQRVkX",
        //    "010460718936130221V=lUop5MgVHV=",
        //    "0104607189361319213eOpd8JLeL300",
        //    "010460718936131921d+MkqM\"L9c\"Va",
        //    "010460718936131921hUK2TUlrgVM_d",
        //    "010460718936131921NTgZgD>!!St9Q",
        //    "010460718936131921RR<-9czU';bg<",
        //    "010460718936131921Uo__Hg-kQRk4o",
        //    "010460718936131921VBjOB+I<nuh0C"
        //};
    }
}
