using System.Text.Json.Serialization;

namespace bs
{
    public class OrderTiresSn : OrderSn
    {
        private IConfiguration config;
        [JsonConstructor]
        public OrderTiresSn(string contactPerson, string releaseMethodType, string createMethodType)
        {
            this.contactPerson = contactPerson;
            this.releaseMethodType = releaseMethodType;
            this.createMethodType = createMethodType;
        }
        public OrderTiresSn(IConfiguration conf)
        {
            config = conf;
            contactPerson = conf.GetValue<string>("ContactPerson");
            releaseMethodType = "IMPORT";
            createMethodType = "SELF_MADE";
        }
        public string contactPerson { get; set; }
        public string releaseMethodType { get; set; }
        public string createMethodType { get; set; }
        public string productionOrderId { get; set; } = null;
        [JsonIgnore]
        public override int ProductGroup => 7;
        [JsonIgnore]
        public override string Url => $"tires/orders?omsId={config.GetValue<string>("omsId")}";
        [JsonIgnore]
        public override string Extention => "tires";
        [JsonIgnore]
        public override string PingUrl => $"tires/ping?omsId={config.GetValue<string>("omsId")}";
        public override bool Equals(object obj)
        {
            if (!(obj is OrderTiresSn))
                return false;

            var p = (OrderTiresSn)obj;
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
