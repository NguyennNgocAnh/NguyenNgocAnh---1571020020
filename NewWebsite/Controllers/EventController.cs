using Microsoft.AspNetCore.Mvc;
using Website.Model.Entities;
using Website.Repository;

namespace NewWebsite.Controllers
{
    public class EventController : Controller
    {
        private readonly EventRepository eventRepo;

        public EventController()
        {
            eventRepo = new EventRepository();
        }

        // Hiển thị danh sách sự kiện
        public IActionResult Index()
        {
            var events = eventRepo.GetAll().ToList();
            return View(events);
        }

        // Hiển thị form tạo mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo mới
        [HttpPost]
        public IActionResult Create(Event ev)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    eventRepo.Insert(ev);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(ev);
        }

        // Hiển thị form sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ev = eventRepo.GetById(id);
            if (ev == null)
            {
                return NotFound();
            }
            return View(ev);
        }

        // Xử lý sửa
        [HttpPost]
        public IActionResult Edit(Event ev)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    eventRepo.Update(ev);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(ev);
        }

        // Xóa sự kiện
        public IActionResult Delete(int id)
        {
            eventRepo.Delete(id);
            return RedirectToAction("Index");
        }
        public IActionResult DownloadPdf(int id)
        {
            var ev = eventRepo.GetById(id);
            if (ev == null)
                return NotFound();

            // Tạo nội dung PDF
            var content = $"Sự kiện: {ev.EventName}\n\nMô tả: {ev.Description}";

            // Tạo file PDF (ví dụ đơn giản với byte[])
            var pdfBytes = System.Text.Encoding.UTF8.GetBytes(content);

            return File(pdfBytes, "application/pdf", $"sukien_{ev.EventId}.pdf");
        }

    }
}
