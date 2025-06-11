using Microsoft.AspNetCore.Mvc;
using Website.Repository;
using System.Linq;
using Website.Model.Entities;

namespace NewWebsite.Controllers
{
    public class SearchController : Controller
    {
        private readonly NewsRepository newsRepo;
        private readonly MarketPricesRepository marketRepo;
        private readonly StockRepository stockRepo;
        private readonly StockContext _context;

        public SearchController()
        {
            newsRepo = new NewsRepository();
            marketRepo = new MarketPricesRepository();
            stockRepo = new StockRepository();
            _context = new StockContext();
        }

        // Action tìm kiếm
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Nếu không có từ khóa tìm kiếm thì trả về View mặc định (Index)
                ViewData["Message"] = "Vui lòng nhập từ khóa tìm kiếm.";
                return View("Index", new List<Website.Model.Entities.News>());
            }

            // Lọc danh sách tin tức theo từ khóa tìm kiếm
            var newsList = newsRepo.GetAll()
                .Where(p => !string.IsNullOrEmpty(p.Title) && p.Title.Contains(keyword))
                .ToList();

            // Nếu không có kết quả tìm thấy, hiển thị thông báo
            if (newsList.Count == 0)
            {
                ViewData["Message"] = "Không tìm thấy kết quả phù hợp.";
            }

            // Trả về kết quả tìm kiếm hoặc danh sách trống nếu không có kết quả
            return View("Index", newsList);
        }


        // Action Index nếu không có từ khóa tìm kiếm
        public IActionResult Index()
        {
            return View();
        }
    }
}
