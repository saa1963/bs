using System.Diagnostics.CodeAnalysis;

namespace bs
{
    public class OrderResponse
    {
        public string? omsId { get; set; }
        [NotNull]
        public string? orderId { get; set; }
        public long expectedCompleteTimestamp { get; set; }
    }
}
