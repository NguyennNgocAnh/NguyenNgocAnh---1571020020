using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Model.Entities;
using System.Threading.Tasks;
using NewWebsite.Filters;
using Website.Repository;
using System.Net.Mail;

namespace NewWebsite.Controllers
{
    [SessionAuthorize]
    public class AlertViewController : Controller
    {
        private readonly StockContext _context;
        private readonly IEmailService _emailService;

        public AlertViewController(StockContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Alert/Create
        public IActionResult Create()
        {
            ViewBag.StockList = _context.Stocks.ToList();
            return View();
        }

        // POST: Alert/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Alert alert)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StockList = _context.Stocks.ToList();
                return View(alert);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                ModelState.AddModelError("", "Vui lòng đăng nhập để đặt cảnh báo.");
                ViewBag.StockList = _context.Stocks.ToList();
                return View(alert);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == userId.Value);
            if (customer == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin người dùng.");
                ViewBag.StockList = _context.Stocks.ToList();
                return View(alert);
            }
            if (!alert.TriggeredTime.HasValue)
            {
                ModelState.AddModelError("TriggeredTime", "Vui lòng chọn ngày cảnh báo.");
                ViewBag.StockList = _context.Stocks.ToList();
                return View(alert);
            }

            alert.CustomerId = customer.CustomerId;
            alert.IsTriggered = false;
            alert.TriggerMessage = string.Empty;

            try
            {
                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cảnh báo đã được đặt thành công!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi khi lưu cảnh báo.");
                ViewBag.StockList = _context.Stocks.ToList();
                return View(alert);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Alert/Index
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == userId.Value);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var alerts = await _context.Alerts
                .Include(a => a.Stock)
                .Where(a => a.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(alerts);
        }
        // GET: Alert/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == userId.Value);
            if (customer == null || alert.CustomerId != customer.CustomerId)
            {
                return Forbid();
            }

            ViewBag.StockList = _context.Stocks.ToList();
            return View(alert);
        }

        // POST: Alert/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Alert alert)
        {
            if (id != alert.AlertId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.StockList = _context.Stocks.ToList();
                return View(alert);
            }

            var existingAlert = await _context.Alerts.FindAsync(id);
            if (existingAlert == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == userId.Value);
            if (customer == null || existingAlert.CustomerId != customer.CustomerId)
            {
                return Forbid();
            }

            existingAlert.AlertName = alert.AlertName;
            existingAlert.PriceThresholdUp = alert.PriceThresholdUp;
            existingAlert.PriceThresholdDown = alert.PriceThresholdDown;
            existingAlert.PercentChange = alert.PercentChange;
            existingAlert.VolumeThreshold = alert.VolumeThreshold;
            existingAlert.StockId = alert.StockId;
            existingAlert.IsTriggered = false;
            existingAlert.TriggeredTime = null;
            existingAlert.TriggerMessage = string.Empty;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi khi cập nhật cảnh báo.");
                ViewBag.StockList = _context.Stocks.ToList();
                return View(alert);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Alert/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var alert = await _context.Alerts
                .Include(a => a.Stock)
                .FirstOrDefaultAsync(a => a.AlertId == id);

            if (alert == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == userId.Value);
            if (customer == null || alert.CustomerId != customer.CustomerId)
            {
                return Forbid();
            }

            return View(alert);
        }

        // POST: Alert/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == userId.Value);
            if (customer == null || alert.CustomerId != customer.CustomerId)
            {
                return Forbid();
            }

            try
            {
                _context.Alerts.Remove(alert);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi khi xóa cảnh báo.");
                return View(alert);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}