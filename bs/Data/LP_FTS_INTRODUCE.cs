using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace bs.Data
{
    public class LP_FTS_INTRODUCE
    {
        public string? trade_participant_inn { get; set; }
        public string? declaration_number { get; set; }
        public string? declaration_date { get; set; }
        [NotNull]
        public List<Product>? products_list { get; set; }
        public override bool Equals(object? obj)
        {
            if (!(obj is LP_FTS_INTRODUCE))
                return false;

            var p = (LP_FTS_INTRODUCE)obj;
            return p.trade_participant_inn == trade_participant_inn && p.declaration_number == declaration_number
                && p.declaration_date == declaration_date
                && p.products_list.OrderBy(s => s.cis).ToList().SequenceEqual(products_list.OrderBy(s => s.cis).ToList());
        }
        public override int GetHashCode()
        {
            int hash = 15;
            products_list.OrderBy(s => s.cis).ToList().ForEach(s => hash ^= s.cis.GetHashCode());
            return hash;
        }
    }
}
