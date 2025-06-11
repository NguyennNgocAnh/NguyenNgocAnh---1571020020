using Microsoft.AspNetCore.Mvc;
using Website.Model.Entities;
using Website.Repository;

namespace Website.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class EventController : Controller
    {
        private EventRepository eventRepo;

        public EventController()
        {
            eventRepo = new EventRepository();
        }

        public IActionResult Index()
        {
            var events = eventRepo.GetAll().ToList(); 
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Event eventModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    eventRepo.Insert(eventModel);
                    return Redirect("/Administrator/Event/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(eventModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var eventModel = eventRepo.GetById(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        [HttpPost]
        public IActionResult Edit(Event eventModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    eventRepo.Update(eventModel);
                    return Redirect("/Administrator/Event/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(eventModel);
        }

        public IActionResult Delete(int id)
        {
            eventRepo.Delete(id);
            return Redirect("/Administrator/Event/Index");
        }
    }
}
