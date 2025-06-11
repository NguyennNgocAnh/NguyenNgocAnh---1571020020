using Microsoft.AspNetCore.Mvc;
using Website.Model.Entities;

namespace NewWebsite.Controllers
{
    public class NotificationController : Controller
    {
        private readonly StockContext _context;

        public NotificationController(StockContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Title,Content,Rating")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Index", notification);
        }

    }
}
