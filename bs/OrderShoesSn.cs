using System.Text.Json.Serialization;

namespace bs
{
    public class OrderShoesSn : OrderSn
    {
        private IConfiguration? config;
        [JsonConstructor]
        public OrderShoesSn(string contactPerson, string releaseMethodType, string createMethodType)
        {
            this.contactPerson = contactPerson;
            this.releaseMethodType = releaseMethodType;
            this.createMethodType = createMethodType;

        }
        public OrderShoesSn(IConfiguration conf)
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
        public override int ProductGroup => 1;
        [JsonIgnore]
        public override string Url => $"shoes/orders?omsId={config.GetValue<string>("omsId")}";
        [JsonIgnore]
        public override string Extention => "shoes";
        [JsonIgnore]
        public override string PingUrl => $"shoes/ping?omsId={config.GetValue<string>("omsId")}";
        public override bool Equals(object? obj)
        {
            if (!(obj is OrderShoesSn))
                return false;

            var p = (OrderShoesSn)obj;
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
