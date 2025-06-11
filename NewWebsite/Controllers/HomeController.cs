using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NewWebsite.Filters;
using NewWebsite.Models;
using Website.Model.Entities;
using Website.Repository;

namespace NewWebsite.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StockContext _context;
        private readonly CategoriesRepository categoriesRepo;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = new StockContext();
            categoriesRepo = new CategoriesRepository();
        }

        public IActionResult Index()
        {
            // Lấy danh sách tất cả các Category (bao gồm cả CategoryId và CategoryName)
            var categories = _context.Categories
                .ToList();  // Lấy cả thông tin CategoryId và CategoryName

            var newsByCategory = new Dictionary<int, List<News>>();

            foreach (var cat in categories)
            {
                var news = _context.News
                    .Where(n => n.CategoryId == cat.CategoryId)
                    .OrderByDescending(n => n.PublishDate)
                    .Take(3)
                    .ToList();

                newsByCategory[cat.CategoryId] = news;
            }

            var newsList = newsByCategory.SelectMany(kvp => kvp.Value).ToList();

            var model = new Tuple<List<Category>, List<News>>(categories, newsList);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
