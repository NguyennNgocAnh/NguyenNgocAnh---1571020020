using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class HighlightedNews
    {
        public int HighlightId { get; set; }
        public string? Title { get; set; }
        public int? NewsId { get; set; }

        public virtual News? News { get; set; }
    }
}
