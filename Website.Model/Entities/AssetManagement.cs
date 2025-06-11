using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class AssetManagement
    {
        public int AssetId { get; set; }
        public string AssetCode { get; set; } = null!;
        public string AssetName { get; set; } = null!;
        public string Action { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? ProfitLoss { get; set; }
        public DateTime? ActionDate { get; set; }
        public string? Note { get; set; }
        public int? CustomerId { get; set; }
        public int? StockId { get; set; }

        public int UserId { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Stock? Stock { get; set; }
        public virtual User? User { get; set; }
    }
}
