using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using bs.Data;
using Microsoft.Extensions.Configuration;

namespace bs
{
    public class ProgramHttp_TrueApi_v4
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static HttpClient GetHttpClientTrueApi(IConfiguration cfg)
        {
            return GetHttpClient(cfg.GetValue<string>("baseAddressTrueApi"), cfg);
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
        internal static async Task<string> _AuthenticateMeAsync(IConfiguration cfg, bool isSuz)
        {
            if (isSuz)
            {
                if (_authToken != null && _authTokenCreated.HasValue && (_authTokenCreated.Value + TimeSpan.FromMinutes(570) > DateTime.Now))
                {
                    return _authToken;
                }
            }
            var httpClient = GetHttpClientTrueApi(cfg);
            var response = await httpClient.GetAsync("auth/key");
            var content = await response.Content.ReadAsStringAsync();
            var o = JsonSerializer.Deserialize<auth_key_response>(content);

            if (o.uuid == null || o.data == null)
            {
                throw new SaException("Запрос auth/key не возвратил данные");
            }
            var signedData = HSignManaged.HSign.AttachedSign(Encoding.UTF8.GetBytes(o.data), cfg.GetValue<string>("nameCert"), cfg.GetValue<string>("nameCertSurname"));
            var oContent = new auth_simpleSignIn_request()
            {
                uuid = o.uuid,
                data = Convert.ToBase64String(signedData)
            };
            auth_simpleSignIn_response? resp = null;
            resp =
                await AuthenticatePostApiAsync<auth_simpleSignIn_request, auth_simpleSignIn_response>
                    (oContent,
                    "auth/simpleSignIn" + (isSuz ? "/" + cfg.GetValue<string>("omsConnection") : ""),
                    cfg);
            if (resp.token != null)
            {
                if (isSuz)
                {
                    _authToken = resp.token;
                    _authTokenCreated = DateTime.Now;
                }
                return resp.token;
            }
            else
                throw new SaException("Аутентификация не пройдена. Токен не получен.");
        }
        private static async Task<string> AuthenticateMeAsync(IConfiguration cfg)
        {
            return await _AuthenticateMeAsync(cfg, false);
        }
        private static string? _authToken = null;
        private static DateTime? _authTokenCreated = null;
        private static async Task<U> AuthenticatePostApiAsync<T, U>(T req, string url, IConfiguration cfg)
        {
            var httpClient = GetHttpClientTrueApi(cfg);
            var sContent = JsonSerializer.Serialize<T>(req);
            var content = new StringContent(sContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var s = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<U>(s) ?? throw new Exception("Ошибка десериализации объекта.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("401 Неавторизованный доступ.\r\n" + await response.Content.ReadAsStringAsync());
            }
            else
            {
                var s = await response.Content.ReadAsStringAsync();
                throw new SaException($"{response.StatusCode} {s}");
            }
        }

        internal static async Task<string> Post2TrueApiAsync2<T>(T req, string url, IConfiguration cfg)
        {
            var authToken = await AuthenticateMeAsync(cfg);
            var httpClient = GetHttpClientTrueApi(cfg);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);
            var sContent = JsonSerializer.Serialize(req);
            var content = new StringContent(sContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var s = await response.Content.ReadAsStringAsync();
                return s;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("401 Неавторизованный доступ.\r\n" + await response.Content.ReadAsStringAsync());
            }
            else
            {
                var s = await response.Content.ReadAsStringAsync();
                throw new SaException($"{response.StatusCode} {s}");
            }
        }
        internal static async Task<Tuple<int, string>> IsAcceptedAsync(string docid, IConfiguration cfg, ApiResponse rt)
        {
            //var Config = Cfg.getInstance();
            var authToken = await AuthenticateMeAsync(cfg);
            var httpClient = GetHttpClientTrueApi(cfg);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);
            var response = await httpClient.GetAsync($"doc/{docid}/info");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Logger.Info("IsAcceptedAsync response:");
                Logger.Info(content);
                var resp = JsonSerializer.Deserialize<statusdoc_response_v4>(content);
                if (resp.status == "CHECKED_OK")
                    return new Tuple<int, string>(1, "");
                else if (new List<string>{
                    "CHECKED_NOT_OK",
                    "PARSE_ERROR"}.Contains(resp.status ?? ""))
                {
                    string defRt = $"Документ {docid} не загружен.";
                    string sRt = "";
                    if (resp.errors is not null)
                    {
                        foreach (var error in resp.errors)
                        {
                            sRt += error + "\n";
                        }
                    }
                    return new Tuple<int, string>(2, sRt == "" ? defRt : sRt);
                }
            }
            var err = $"Ошибка обработки запроса. Статус {response.StatusCode}";
            Logger.Error(err);
            return new Tuple<int, string>(0, err);
        }
        public static void WriteError2(ErrorResponse errorObject)
        {
            if (errorObject.fieldErrors != null)
                foreach (var o in errorObject.fieldErrors)
                {
                    Logger.Error($"{o.fieldError} {o.fieldName}");
                }
            if (errorObject.globalErrors != null)
                foreach (var o in errorObject.globalErrors)
                {
                    Logger.Error($"{o.errorCode} {o.error}");
                }
        }
    }
}
