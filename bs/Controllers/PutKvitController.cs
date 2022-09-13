using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace bs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PutKvitController : ControllerBase
    {
        private readonly ILogger<PutKvitController> Logger;
        private IConfiguration Cfg;
        private PutKvitController() { }
        public PutKvitController(IConfiguration cfg, ILogger<PutKvitController> _logger)
        {
            Cfg = cfg;
            Logger = _logger;
        }
        [HttpPut]
        //[Authorize(Policy = "Put")]
        [AllowAnonymous]
        public async Task<ApiResponse> Put()
        {
            var rt = new ApiResponse();
            try
            {

                var docids = new List<Guid>();
                using (var cn = new SqlConnection(Cfg.GetConnectionString("cn_put")))
                {
                    cn.Open();
                    SqlCommand cmd;
                    cmd = new SqlCommand("sp_Mark_RUC_Query", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", "select_docids");
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            docids.Add(dr.GetGuid(0));
                        }
                    }
                    if (docids.Count == 0) throw new SaException("Список идентификатор пуст.");
                    foreach (var o in docids)
                    {
                        await GetReceipt1(o.ToString(), cn, rt);
                        Thread.Sleep(new TimeSpan(0, 0, 10));
                    }
                    rt.status = 0;
                    rt.AddMsg("Ok");
                    return rt;
                }
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
        private async Task GetReceipt1(string doc, SqlConnection cn, ApiResponse rt)
        {
            (int res, string err) = await ProgramHttp_TrueApi_v4.IsAcceptedAsync(doc.ToString(), Cfg, rt);
            var cmd = new SqlCommand("dbo.sp_Mark_RUC_Query", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "update_kvit");
            cmd.Parameters.AddWithValue("@DocUid", doc);
            cmd.Parameters.AddWithValue("@p_status", res);
            if (res == 1)
            {
                cmd.ExecuteNonQuery();
                Logger.LogInformation(rt.AddMsg($"Документ {doc} обработан. Товары введены в оборот."));
            }
            else if (res == 2) // p_status = 2, DocId сбрасывается, КМ опять попадет в новый документ по вводу в оборот
            {
                cmd.ExecuteNonQuery();
                Logger.LogError(rt.AddMsg(err));
                Logger.LogError(rt.AddMsg($"Ошибка обработки документа {doc}. КМ попадут в новый документ по вводу в оборот."));
            }
            else
            {
                Logger.LogInformation(rt.AddMsg($"Документ {doc} не найден. Попробуйте через некоторое время."));
            }
        }
    }
}
