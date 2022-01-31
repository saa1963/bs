using System.Diagnostics.CodeAnalysis;

namespace bs
{
    public class GetKmResponse
    {
        public string? omsId { get; set; }
        [NotNull]
        public string[]? codes { get; set; }
        [NotNull]
        public string? blockId { get; set; }
    }
}
