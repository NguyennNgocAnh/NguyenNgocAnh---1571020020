using Microsoft.AspNetCore.Mvc;
using NewWebsite.Filters;
using Website.Repository;

namespace NewWebsite.Controllers
{
    [SessionAuthorize]
    public class PriceHistoryController : Controller
    {
        private readonly PriceHistoryRepository priceHistoryRepo;
        public PriceHistoryController()
        {
            priceHistoryRepo = new PriceHistoryRepository();
        }
        public IActionResult Index()
        {
            var histories = priceHistoryRepo.GetAll().ToList(); 
            return View(histories); 
        }
    }
}
