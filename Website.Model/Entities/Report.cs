using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Report
    {
        public int ReportId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? FilePath { get; set; }
    }
}
