using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace bs
{
    public class ProgramHttp_Suz_v2
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static HttpClient GetHttpClientSuz(IConfiguration cfg)
        {
            return GetHttpClient(cfg.GetValue<string>("baseAddress"), cfg);
        }
        private static HttpClient GetHttpClient(string baseAddress, IConfiguration cfg)
        {
            HttpClient httpClient;
            if (cfg.GetValue<bool>("messageLog"))
                httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));
            else
                httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }
        //private static string? _authToken = null;
        //private static DateTime? _authTokenCreated = null;
        private static async Task<string> AuthenticateMeSuzAsync(IConfiguration cfg)
        {
            return await ProgramHttp_TrueApi_v4._AuthenticateMeAsync(cfg, true);
        }
        private static byte[] GetContentForSigning(string sContent, string url)
        {
            string s = sContent;
            return Encoding.UTF8.GetBytes(s);
        }
        public static async Task<Tuple<OrderResponse?, ErrorResponse?>?> Post2ApiAsync(string sContent, string url, IConfiguration cfg)
        {
            var msg = GetContentForSigning(sContent, url);
            var certName = cfg.GetValue<string>("nameCert");
            var surName = cfg.GetValue<string>("nameCertSurname");
            var signedData = Convert.ToBase64String(HSignManaged.HSign.Sign(msg, certName, surName));
            var authToken = await AuthenticateMeSuzAsync(cfg);
            var h = GetHttpClientSuz(cfg);
            h.DefaultRequestHeaders.Add("X-Signature", signedData);
            h.DefaultRequestHeaders.Add("clientToken", authToken);
            var content = new StringContent(sContent, Encoding.UTF8, "application/json");
            var response = await h.PostAsync(url, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var s = await response.Content.ReadAsStringAsync();
                var rt = JsonSerializer.Deserialize<OrderResponse>(s);
                if (rt != null)
                    return new Tuple<OrderResponse?, ErrorResponse?>(rt, null);
                else
                    return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var s = await response.Content.ReadAsStringAsync();
                var rt = JsonSerializer.Deserialize<ErrorResponse>(s);
                if (rt != null)
                    return new Tuple<OrderResponse?, ErrorResponse?>(null, rt);
                else
                    return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                var s = await response.Content.ReadAsStringAsync();
                var rt = JsonSerializer.Deserialize<ErrorResponse>(s);
                if (rt != null)
                    return new Tuple<OrderResponse?, ErrorResponse?>(null, rt);
                else
                    return null;
            }
            else
            {
                var s = await response.Content.ReadAsStringAsync();
                throw new Exception($"{response.StatusCode} {s}");
            }
        }
        public static async Task<BufferStatusResponse> _BufferStatusAsync(int productgroup, string orderId, string gtin, IConfiguration cfg)
        {
            string omsId = cfg.GetValue<string>("omsId");
            string url = $"{StringExtension(productgroup)}/buffer/status?omsId={omsId}&orderId={orderId}&gtin={gtin}";

            var authToken = await AuthenticateMeSuzAsync(cfg);
            var h = GetHttpClientSuz(cfg);
            h.DefaultRequestHeaders.Add("clientToken", authToken);
            var response = await h.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var rt = JsonSerializer.Deserialize<BufferStatusResponse>(content) ?? throw new SaException("");
                return rt;
            }
            else
                throw new SaException($"Ошибка вызова.{response.StatusCode} {response.ReasonPhrase}");
        }
        public static async Task<Tuple<int, string?>> BufferStatusAsync1(int productgroup, string orderId, string gtin, IConfiguration cfg)
        {
            var rt = await _BufferStatusAsync(productgroup, orderId, gtin, cfg);
            if (rt.bufferStatus == "PENDING")
            {
                Logger.Info("Данные не готовы");
                return new Tuple<int, string?>(2, "PENDING");
            }
            else
            {
                if (rt.bufferStatus == "ACTIVE")
                    return new Tuple<int, string?>(0, "");
                else if (rt.bufferStatus == "REJECTED")
                {
                    Logger.Info(rt.rejectionReason);
                    return new Tuple<int, string?>(1, rt.rejectionReason);
                }
                else
                {
                    Logger.Info($"Буфер КМ не доступен. Статус {rt.bufferStatus}");
                    return new Tuple<int, string?>(2, rt.bufferStatus);
                }
            }
        }
        private static string StringExtension(int productgroup)
        {
            switch (productgroup)
            {
                case 1: return "shoes";
                case 7: return "tires";
                case 10: return "lp";
                default: throw new SaException("Неизвестная товарная группа");
            }
        }
        public static async Task<int> GetQuantityKmInBufferAsync(int productgroup, string orderId, string gtin, IConfiguration cfg)
        {
            var rt = await _BufferStatusAsync(productgroup, orderId, gtin, cfg);
            if (rt.bufferStatus == "PENDING") // функция не вызывается до проверки состояния буфера на шаге 2 (это невероятный вариант)
            {
                throw new SaException("Получение количества кодов в буфере до его проверки");
            }
            else
            {
                if (rt.bufferStatus == "ACTIVE")
                    return rt.leftInBuffer;
                else if (rt.bufferStatus == "REJECTED")
                {
                    Logger.Info(rt.rejectionReason);
                    return 0;
                }
                else
                {
                    Logger.Info($"Буфер КМ не доступен. Статус {rt.bufferStatus}");
                    return 0;
                }
            }
        }
        public static async Task<Tuple<GetKmResponse?, List<string>>> GetKmAsync(int productgroup, string orderId, string gtin, int qty, string blockId, IConfiguration cfg)
        {
            string omsId = cfg.GetValue<string>("omsId");
            string url0 = $"/codes?omsId={omsId}&orderId={orderId}&gtin={gtin}&quantity={qty}&lastBlockId={blockId}";
            string url = $"{StringExtension(productgroup)}" + url0;
            var authToken = await AuthenticateMeSuzAsync(cfg);
            var h = GetHttpClientSuz(cfg);
            h.DefaultRequestHeaders.Add("clientToken", authToken);
            var response = await h.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var errs = new List<string>();
            if (response.IsSuccessStatusCode)
            {
                var rt = JsonSerializer.Deserialize<GetKmResponse>(content);
                return Tuple.Create(rt, errs);
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                if (errorResponse != null)
                {
                    errs = WriteError2(errorResponse);
                }
                return Tuple.Create<GetKmResponse?, List<string>>(null, errs);
            }
        }
        public static List<string> WriteError2(ErrorResponse errorObject)
        {
            var rt = new List<string>();
            if (errorObject.fieldErrors != null)
                foreach (var o in errorObject.fieldErrors)
                {
                    Logger.Error($"{o.fieldError} {o.fieldName}");
                    rt.Add($"{o.fieldError} {o.fieldName}");
                }
            if (errorObject.globalErrors != null)
                foreach (var o in errorObject.globalErrors)
                {
                    Logger.Error($"{o.errorCode} {o.error}");
                    rt.Add($"{o.errorCode} {o.error}");
                }
            return rt;
        }
    }
}
