using Microsoft.AspNetCore.Mvc;
using Website.Model.Entities;
using Website.Repository;

namespace NewWebsite.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsRepository newRepo;
        private readonly DetailRepository detailRepo;
        private readonly CategoriesRepository categoriesRepo;
        private readonly CommentRepository commentRepo;
        private readonly StockContext _context;

        public NewsController()
        {
            newRepo = new NewsRepository();
            detailRepo = new DetailRepository();
            categoriesRepo = new CategoriesRepository();
            commentRepo = new CommentRepository();
            _context = new StockContext();
        }

        public IActionResult Index()
        {
            var news = newRepo.GetAll().ToList();
            return View(news);
        }

        public IActionResult Detail(int id)
        {
            var detail = detailRepo.GetAll().FirstOrDefault(p => p.NewsId == id);

            if (detail == null)
            {
                return NotFound();
            }
            var model = _context.News.FirstOrDefault(n => n.NewsId == id);
            if (model == null) return NotFound();

            var content = model.Content;
            var contentParts = new List<string>();
            var currentIndex = 0;
            var maxLength = 396;

            while (currentIndex < content.Length)
            {
                var part = content.Substring(currentIndex, Math.Min(maxLength, content.Length - currentIndex));
                contentParts.Add(part);
                currentIndex += part.Length;
            }

            for (int i = 0; i < contentParts.Count; i++)
            {
                if (i < 5)
                {
                    var imageName = $"Image{i + 1}";
                    var imageUrl = model.GetType().GetProperty(imageName)?.GetValue(model)?.ToString();
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        contentParts.Insert(i * 2 + 1, $"<img src='/img/news/{imageUrl}' alt='Ảnh tin tức {i + 1}' class='img-fluid' />");
                    }
                }
            }

            ViewBag.ContentParts = contentParts;


            var categories = categoriesRepo.GetAll();
            ViewBag.Categories = categories;

            var comment = commentRepo.GetAll()
                .Where(t => t.NewsId == id)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();
            ViewBag.Comments = comment;

            return View(detail);
        }

        [HttpPost]
        public IActionResult AddComment(int NewsId, string Content)
        {
            var comment = new Comment
            {
                NewsId = NewsId,
                CustomerId = 1,
                Content = Content,
                CreatedAt = DateTime.Now
            };

            commentRepo.Insert(comment);
            return RedirectToAction("Detail", new { id = NewsId });
        }

        public IActionResult Categories(int id)
        {
            var newsList = newRepo.GetAll()
                .Where(t => t.CategoryId == id)
                .OrderByDescending(t => t.PublishDate)
                .ToList();

            var categoryName = categoriesRepo.GetById(id)?.CategoryName ?? "Danh mục";

            ViewBag.CategoryName = categoryName;
            ViewBag.CategoryId = id;
            return View("Index", newsList);
        }
        [HttpGet]
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ViewBag.Keyword = "";
                return View("Index", new List<News>());
            }

            var results = newRepo.GetAll()
                .Where(n => n.Title.Contains(keyword) || n.Content.Contains(keyword))
                .OrderByDescending(n => n.PublishDate)
                .ToList();

            ViewBag.Keyword = keyword;
            return View("Index", results);
        }
    }
}
