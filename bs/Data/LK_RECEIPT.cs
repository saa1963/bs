namespace bs.Data
{
    public class LK_RECEIPT
    {
        public string action { get; set; }
        public string action_date { get; set; }
        public string document_date { get; set; }
        public string document_number { get; set; }
        public string document_type { get; set; }
        public string inn { get; set; }
        public string primary_document_custom_name { get; set; }
        public List<LK_RECEIPT_PRODUCTS> products { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is LK_RECEIPT))
                return false;

            var p = (LK_RECEIPT)obj;
            return p.action == action && p.action_date == action_date
                && p.document_date == document_date && p.document_number == document_number
                && p.document_type == document_type && p.inn == inn
                && p.primary_document_custom_name == primary_document_custom_name
                && p.products.OrderBy(s => s.cis).ToList().SequenceEqual(products.OrderBy(s => s.cis).ToList());
        }
        public override int GetHashCode()
        {
            int hash = 17;
            products.ForEach(p => hash ^= p.cis.GetHashCode());
            return hash;
        }
    }
    public class LK_RECEIPT_PRODUCTS
    {
        public string cis { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is LK_RECEIPT_PRODUCTS))
                return false;

            var p = (LK_RECEIPT_PRODUCTS)obj;
            return p.cis == cis;
        }
        public override int GetHashCode()
        {
            return cis.GetHashCode();
        }
    }
}
