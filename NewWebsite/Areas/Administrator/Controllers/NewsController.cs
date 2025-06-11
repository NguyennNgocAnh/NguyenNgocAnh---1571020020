using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Website.Model.Entities;
using Website.Repository;

namespace NewWebsite.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class NewsController : Controller
    {
        private readonly NewsRepository newsRepo;
        private readonly StockContext _context;

        public NewsController()
        {
            newsRepo = new NewsRepository();
            _context = new StockContext();
        }

        public IActionResult Index()
        {
            var news = newsRepo.GetAll().ToList();
            return View(news);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

            ViewBag.CategoriesList = categories;

            return View();
        }

        [HttpPost]
        public IActionResult Create(News news, IFormFile AnhFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (AnhFile != null)
                    {
                        news.Image1 = FileUploadHelper.UploadImage(AnhFile);
                    }

                    newsRepo.Insert(news);
                    return Redirect("/Administrator/News/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(news);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

            ViewBag.CategoriesList = categories;

            var news = newsRepo.GetById(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [HttpPost]
        public IActionResult Edit(News news, IFormFile AnhFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (AnhFile != null)
                    {
                        news.Image1 = FileUploadHelper.UploadImage(AnhFile);
                    }

                    newsRepo.Update(news);
                    return Redirect("/Administrator/News/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(news);
        }

        public IActionResult Delete(int id)
        {
            newsRepo.Delete(id);
            return Redirect("/Administrator/News/Index");
        }

        // Helper xử lý upload ảnh
        public static class FileUploadHelper
        {
            public static string UploadImage(IFormFile file, string folder = "img/news")
            {
                if (file == null || file.Length == 0) return null;

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return uniqueFileName;
            }
        }
    }
}
