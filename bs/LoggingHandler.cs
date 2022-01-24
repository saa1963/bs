namespace bs
{
    public class LoggingHandler : DelegatingHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string log = "";
            log += "Request:\r\n";
            log += request.ToString() + "\r\n";
            if (request.Content != null)
            {
                log += await request.Content.ReadAsStringAsync() + "\r\n";
            }
            log += "\r\n";

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            log += "Response:\r\n";
            log += response.ToString() + "\r\n";
            if (response.Content != null)
            {
                log += await response.Content.ReadAsStringAsync() + "\r\n";
            }
            log += "\r\n";
            Logger.Info(log);

            return response;
        }
    }
}
