using System.Diagnostics.CodeAnalysis;

namespace bs.Data
{
    public class WithDrawalInput
    {
        public string? ndoc { get; set; }
        public DateTime ddoc { get; set; }
        public string? km { get; set; }
        [NotNull]
        public string? tg { get; set; }
        public string? article { get; set; }
    }
}
