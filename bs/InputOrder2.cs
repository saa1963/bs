using System.Diagnostics.CodeAnalysis;

namespace bs
{
    public class InputOrder2
    {
        [NotNull]
        public string? article { get; set; }
        [NotNull]
        public string? gtin { get; set; }
        [NotNull]
        public string? sn { get; set; }
        public int tg { get; set; }
    }
}
