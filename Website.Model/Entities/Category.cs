using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Category
    {
        public Category()
        {
            News = new HashSet<News>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryType { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
