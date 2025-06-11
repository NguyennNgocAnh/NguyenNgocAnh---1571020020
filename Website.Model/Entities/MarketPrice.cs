using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class MarketPrice
    {
        public int PriceId { get; set; }
        public DateTime? PriceDate { get; set; }
        public decimal? ReferencePrice { get; set; }
        public decimal? OpenPrice { get; set; }
        public decimal? CeilingPrice { get; set; }
        public decimal? FloorPrice { get; set; }
        public decimal? HighPrice { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? AveragePrice { get; set; }
        public int? MatchedVolume { get; set; }
        public decimal? MatchedPrice { get; set; }
        public int? Buy { get; set; }
        public int? Sell { get; set; }
        public decimal? Change { get; set; }
        public int? StockId { get; set; }

        public virtual Stock? Stock { get; set; }
    }
}
