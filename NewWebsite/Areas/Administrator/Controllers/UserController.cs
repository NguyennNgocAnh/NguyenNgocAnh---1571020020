using Microsoft.AspNetCore.Mvc;
using NuGet.ContentModel;
using Website.Model.Entities;
using Website.Repository;

namespace NewWebsite.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class UserController : Controller
    {
        private UserRepository userRepo;
        public UserController()
        {
            userRepo = new UserRepository();
        }

        public IActionResult Index()
        {
            var user = userRepo.GetAll().ToList();
            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Route("/Administrator/User/Create")]
        [HttpPost]
        public IActionResult Create(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    userRepo.Insert(user);
                    return Redirect("/Administrator/User/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = userRepo.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                user.UserId = userId.Value;
            }
            try
            {
                if (ModelState.IsValid)
                {
                    userRepo.Update(user);
                    return Redirect("/Administrator/User/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return Redirect("/Administrator/User/Index");
        }

        public IActionResult Delete(int id)
        {
            userRepo.Delete(id);
            return Redirect("/Administrator/User/Index");
        }
    }
}
