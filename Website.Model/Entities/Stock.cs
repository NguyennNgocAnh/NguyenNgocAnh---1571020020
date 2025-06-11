using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Stock
    {
        public Stock()
        {
            Alerts = new HashSet<Alert>();
            AssetManagements = new HashSet<AssetManagement>();
            MarketPrices = new HashSet<MarketPrice>();
            PriceHistories = new HashSet<PriceHistory>();
        }

        public int StockId { get; set; }
        public string StockName { get; set; } = null!;
        public string Exchange { get; set; } = null!;
        public int? IndustryId { get; set; }

        public virtual Industry? Industry { get; set; }
        public virtual ICollection<Alert> Alerts { get; set; }
        public virtual ICollection<AssetManagement> AssetManagements { get; set; }
        public virtual ICollection<MarketPrice> MarketPrices { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
    }
}
