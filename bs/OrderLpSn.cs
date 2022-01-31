using System.Text.Json.Serialization;

namespace bs
{
    public class OrderLpSn : OrderSn
    {
        private IConfiguration? config;
        [JsonConstructor]
        public OrderLpSn(string contactPerson, string releaseMethodType, string createMethodType)
        {
            this.contactPerson = contactPerson;
            this.releaseMethodType = releaseMethodType;
            this.createMethodType = createMethodType;
        }
        public OrderLpSn(IConfiguration conf)
        {
            config = conf;
            contactPerson = conf.GetValue<string>("ContactPerson");
            releaseMethodType = "IMPORT";
            createMethodType = "SELF_MADE";
        }
        public string contactPerson { get; set; }
        public string releaseMethodType { get; set; }
        public string createMethodType { get; set; }
        public string? productionOrderId { get; set; } = null;
        [JsonIgnore]
        public override int ProductGroup => 10;
        [JsonIgnore]
        public override string Url => $"lp/orders?omsId={config.GetValue<string>("omsId")}";
        [JsonIgnore]
        public override string Extention => "lp";
        [JsonIgnore]
        public override string PingUrl => $"lp/ping?omsId={config.GetValue<string>("omsId")}";
        public override bool Equals(object? obj)
        {
            if (!(obj is OrderLpSn))
                return false;

            var p = (OrderLpSn)obj;
            return p.contactPerson == contactPerson && p.releaseMethodType == releaseMethodType && p.createMethodType == createMethodType
                && p.products.SequenceEqual(products);
        }
        public override int GetHashCode()
        {
            return products[0].GetHashCode();
        }
        public override void AddProduct(OrderProductSn product)
        {
            products.Add(product);
        }
    }
}
