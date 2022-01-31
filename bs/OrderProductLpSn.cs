namespace bs
{
    public class OrderProductLpSn : OrderProductSn
    {
        public string? exporterTaxpayerId { get; set; }
        public OrderProductLpSn() { }
        public OrderProductLpSn(string? article, string? gtin, int quantity, List<string>? sn)
        {
            this.article = article;
            this.gtin = gtin;
            this.quantity = quantity;
            this.serialNumberType = "SELF_MADE";
            this.templateId = 10;
            this.cisType = "UNIT";
            this.serialNumbers = sn.ToArray();
        }
        public override bool Equals(object? obj)
        {
            if (!(obj is OrderProductLpSn))
                return false;

            var p = (OrderProductLpSn)obj;
            return p.gtin == gtin && p.quantity == quantity
                && p.serialNumberType == serialNumberType && p.templateId == templateId
                && p.cisType == cisType && p.serialNumbers.OrderBy(s => s)
                    .ToList().SequenceEqual(serialNumbers.OrderBy(s => s).ToList());
        }
        public override int GetHashCode()
        {
            int hash = 11;
            foreach (var s in serialNumbers)
            {
                hash ^= s.GetHashCode();
            }
            return hash;
        }
    }
}
