namespace bs
{
    public class InputOrder3
    {
        public string? article { get; set; }
        public string? gtin { get; set; }
        public int ordered { get; set; }
        public int tg { get; set; }
        public List<string>? sn { get; set; } = new List<string>();
    }
}
