using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class User
    {
        public User()
        {
            News = new HashSet<News>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserRole { get; set; } = null!;
        public string? Avatar { get; set; }

        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<AssetManagement> AssetManagements { get; set; }

    }
}
