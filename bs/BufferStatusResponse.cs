namespace bs
{
    public class BufferStatusResponse
    {
        public int availableCodes { get; set; }
        public string? bufferStatus { get; set; }
        public string? gtin { get; set; }
        public int leftInBuffer { get; set; }
        public string? omsId { get; set; }
        public string? orderId { get; set; }
        public List<PoolInfo>? poolInfos { get; set; }
        public bool poolsExhausted { get; set; }
        public string? rejectionReason { get; set; }
        public int totalCodes { get; set; }
        public int totalPassed { get; set; }
        public int unavailableCodes { get; set; }
        public string? expairedDate { get; set; }
    }
}
