using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Industry
    {
        public Industry()
        {
            Stocks = new HashSet<Stock>();
        }

        public int IndustryId { get; set; }
        public string IndustryName { get; set; } = null!;

        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
