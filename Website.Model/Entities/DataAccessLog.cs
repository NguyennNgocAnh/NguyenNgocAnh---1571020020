using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class DataAccessLog
    {
        public int LogId { get; set; }
        public string? DataName { get; set; }
        public DateTime? AccessDate { get; set; }
    }
}
