using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.Model.Entities;

namespace Website.Repository
{
    public class UserRepository : WebsiteRepository<User>
    {
        public object GetByUsername(string? username)
        {
            throw new NotImplementedException();
        }
    }
}
