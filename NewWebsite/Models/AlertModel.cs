using Website.Model.Entities;

namespace NewWebsite.Models
{
    public class AlertModel
    {
        public int AlertId { get; set; }
        public string StockId { get; set; }
        public string? AlertName { get; set; }
        public decimal? PriceThresholdUp { get; set; }
        public decimal? PriceThresholdDown { get; set; }
        public double? PercentChangeUp { get; set; }
        public double? PercentChangeDown { get; set; }
        public int? VolumeThreshold { get; set; }
        public int? CustomerId { get; set; }

        public bool IsTriggered { get; set; } = false;
        public DateTime? TriggeredTime { get; set; }
        public string TriggerMessage { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Stock? Stock { get; set; }
    }
}
