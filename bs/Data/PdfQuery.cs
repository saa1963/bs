using System.Diagnostics.CodeAnalysis;

namespace bs.Data
{
    public class PdfQuery
    {
        public int Id { get; set; }
        [NotNull]
        public string? Заказ { get; set; }
        public string? Артикул { get; set; }
        [NotNull]
        public string? Код { get; set; }
        public DateTime? Последняя_печать { get; set; }
        public bool Печатать { get; set; } = false;
    }
}
