using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace bs.Data
{
    public class Product
    {
        public string cis { get; set; }
        public string packType { get; set; }
        [JsonIgnore]
        public string color { get; set; }
        [JsonIgnore]
        public string productSize { get; set; }
        [JsonIgnore]
        public List<ProductChildren> children { get; set; }
        [JsonIgnore]
        public string GTDNum { get; set; }
        [JsonIgnore]
        public DateTime GTDDate { get; set; }
        [JsonIgnore]
        public string Tg { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is Product))
                return false;

            var p = (Product)obj;
            return p.cis == cis;// && p.packType == packType;
        }
        public override int GetHashCode()
        {
            return cis.GetHashCode();
        }
    }
}
