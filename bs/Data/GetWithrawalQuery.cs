namespace bs.Data
{
    public class GetWithrawalQuery
    {
        public int ID { get; set; }
        public string? Номер_заказа { get; set; }
        public DateTime Дата_заказа { get; set; }
        public string? Код_покупателя { get; set; }
        public string? Наименование_покупателя { get; set; }
        public int Отгружено { get; set; }
        public int Статус_0 { get; set; }
        public int Статус_1 { get; set; }
        public int Статус_2 { get; set; }
    }
}
