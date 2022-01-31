using System.Text.Json.Serialization;

namespace bs
{
    public class OrderProductSn
    {
        [JsonPropertyOrder(1)]
        public int templateId { get; set; }
        [JsonPropertyOrder(2)]
        public string? gtin { get; set; }
        [JsonPropertyOrder(3)]
        public int quantity { get; set; }
        public string? serialNumberType { get; set; }
        [JsonPropertyOrder(4)]
        public string? cisType { get; set; }
        [JsonPropertyOrder(5)]
        public string[] serialNumbers { get; set; } = new string[0];
        [JsonIgnore]
        public string? stickerId { get; set; } = null;
        [JsonIgnore]
        public string? article { get; set; }
    }
}
