namespace bs
{
    public class ErrorResponse
    {
        public List<ProtobeansError>? fieldErrors { get; set; }
        public List<OmsApiGlobalError>? globalErrors { get; set; }
        public bool success { get; set; }
    }
    public class ProtobeansError
    {
        public string? fieldError { get; set; }
        public string? fieldName { get; set; }
    }
    public class OmsApiGlobalError
    {
        public string? error { get; set; }
        public int errorCode { get; set; }
    }
}
