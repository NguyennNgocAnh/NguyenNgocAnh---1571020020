using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }

    }
}
