using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Website.Model.Entities;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly StockContext _context;
    public AccountController()
    {
        _context = new StockContext();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.UserRole);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.UserRole)
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

            // ✔ Điều hướng như cũ
            if (user.UserRole == "Admin")
            {
                return RedirectToAction("Home", "Administrator", new { area = "Admin" });
            }
            else if (user.UserRole == "User")
            {
                return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError("", "Sai thông tin đăng nhập");
        return View();
    }


    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password, string role, string email)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin");
            return View();
        }

        var existingUser = _context.Users.FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
        {
            ModelState.AddModelError("", "Tên tài khoản đã tồn tại");
            return View();
        }
        var existingEmail = _context.Users.FirstOrDefault(u => u.Email == email);
        if (existingEmail != null)
        {
            ModelState.AddModelError("", "Email này đã được sử dụng");
            return View();
        }
        var newUser = new User
        {
            Username = username,
            Password = password,
            UserRole = role,
            Email = email
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
        return RedirectToAction("Login");
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
