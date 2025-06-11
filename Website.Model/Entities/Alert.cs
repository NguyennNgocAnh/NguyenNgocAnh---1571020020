using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Website.Model.Entities
{
    public partial class Alert
    {
        public int AlertId { get; set; }
        public string? AlertName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal? PriceThresholdUp { get; set; }
        public decimal? PriceThresholdDown { get; set; }
        public decimal? PercentChange { get; set; }
        public int? VolumeThreshold { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn cổ phiếu")]
        public int? StockId { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsTriggered { get; set; }
        public DateTime? TriggeredTime { get; set; }
        public string? TriggerMessage { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Stock? Stock { get; set; }

    }
}
