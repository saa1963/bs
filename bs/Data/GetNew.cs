namespace bs.Data
{
    public class GetNew
    {
        public int Id { get; set; }
        public string? Номер_заказа { get; set; }
        public string? Артикул { get; set; }
        public int К_во_на_4_недели { get; set; }
        public int Статус_0 { get; set; }
        public int Статус_1 { get; set; }
        public int Статус_2 { get; set; }
        public int Статус_3 { get; set; }
        public int Рекомендуется_дозаказать { get; set; }
        public int К_во_дозаказать { get; set; }
        public int К_во_больше_4_недель { get; set; }
        public string? Ошибка { get; set; }
    }
}
