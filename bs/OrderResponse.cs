namespace bs
{
    public class OrderResponse
    {
        public string omsId { get; set; }
        public string orderId { get; set; }
        public long expectedCompleteTimestamp { get; set; }
    }
}
