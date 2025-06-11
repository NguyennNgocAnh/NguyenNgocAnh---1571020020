using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Event
    {
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? Description { get; set; }
    }
}
