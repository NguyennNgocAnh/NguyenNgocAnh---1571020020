using Microsoft.AspNetCore.Mvc;
using Website.Model.Entities;
using Website.Repository;

public class UserController : Controller
{
    private readonly StockContext _context;
    private readonly UserRepository userRepo;

    public UserController(StockContext context)
    {
        _context = context;
        userRepo = new UserRepository();
    }

    [HttpGet]
    public IActionResult Profile()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
    [HttpPost]
    public IActionResult UpdateProfile(IFormFile AvatarFile, string Username, string Email)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var user = userRepo.GetById(userId.Value);
        if (user == null)
            return NotFound();

        user.Username = Username;
        user.Email = Email;

        // Nếu có file ảnh mới
        if (AvatarFile != null && AvatarFile.Length > 0)
        {
            var fileName = Path.GetFileName(AvatarFile.FileName);
            var filePath = Path.Combine("wwwroot/uploads", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                AvatarFile.CopyTo(stream);
            }

            user.Avatar = "~/uploads/" + fileName;
        }

        userRepo.Update(user);

        return RedirectToAction("Profile"); // Trở về trang cá nhân
    }

}
