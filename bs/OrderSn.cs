namespace bs
{
    public abstract class OrderSn : RequestBody
    {
        public List<OrderProductSn> products { get; set; } = new List<OrderProductSn>();
        public string? serviceProviderId { get; set; }
        abstract public void AddProduct(OrderProductSn product);
    }
}
