using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class News
    {
        public News()
        {
            Comments = new HashSet<Comment>();
            HighlightedNews = new HashSet<HighlightedNews>();
        }

        public int NewsId { get; set; }
        public string Title { get; set; } = null!;
        public string? SubTitle { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? PublishDate { get; set; }
        public string? Author { get; set; }
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Image3 { get; set; }
        public string? Image4 { get; set; }
        public string? Image5 { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<HighlightedNews> HighlightedNews { get; set; }
    }
}
