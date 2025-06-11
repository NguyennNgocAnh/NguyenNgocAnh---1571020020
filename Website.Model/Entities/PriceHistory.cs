using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class PriceHistory
    {
        public int HistoryId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
        public decimal? ClosePrice { get; set; }
        public decimal? MatchPrice { get; set; }
        public decimal? OpenPrice { get; set; }
        public decimal? HighPrice { get; set; }
        public decimal? LowPrice { get; set; }
        public int? StockId { get; set; }

        public virtual Stock? Stock { get; set; }
    }
}
