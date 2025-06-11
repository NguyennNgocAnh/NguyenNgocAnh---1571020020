using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int? NewsId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual News? News { get; set; }
    }
}
