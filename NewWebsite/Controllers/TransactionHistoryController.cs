using Microsoft.AspNetCore.Mvc;

namespace NewWebsite.Controllers
{
    public class TransactionHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
