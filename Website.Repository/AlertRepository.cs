using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.Model.Entities;

namespace Website.Repository
{
    public class AlertRepository : WebsiteRepository<Alert>
    {
        private readonly StockContext _context;

        public AlertRepository()
        {
            _context = new StockContext();
        }

        public List<Alert> GetAll()
        {
            return _context.Alerts.Include(a => a.Stock).ToList();
        }

        public void Update(Alert alert)
        {
            _context.Alerts.Update(alert);
            _context.SaveChanges();
        }
    }
}
