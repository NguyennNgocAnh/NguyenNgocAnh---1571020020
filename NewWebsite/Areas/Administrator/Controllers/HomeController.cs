using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Website.Model.Entities;

namespace NewWebsite.Areas.Administrator.Controllers
{

    [Area("Administrator")]
    public class HomeController : Controller
    {
        private readonly StockContext _context;

        public HomeController(StockContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var totalUser = _context.Users.Count();
            var totalAsset = _context.AssetManagements.Count();
            var totalAlerts = _context.Alerts.Count();

            ViewBag.totalUser = totalUser;
            ViewBag.totalAsset = totalAsset;
            ViewBag.totalAlerts = totalAlerts;

            return View();
        }
    }
}
