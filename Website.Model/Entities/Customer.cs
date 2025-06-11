using System;
using System.Collections.Generic;

namespace Website.Model.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            Alerts = new HashSet<Alert>();
            AssetManagements = new HashSet<AssetManagement>();
            Comments = new HashSet<Comment>();
        }

        public int CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime? RegisterDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public virtual ICollection<Alert> Alerts { get; set; }
        public virtual ICollection<AssetManagement> AssetManagements { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
