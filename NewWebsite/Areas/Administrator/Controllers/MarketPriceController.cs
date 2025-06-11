using Microsoft.AspNetCore.Mvc;
using Website.Model.Entities;
using Website.Repository;


namespace NewWebsite.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class MarketPriceController : Controller
    {
        private MarketPricesRepository marketRepo;
        public MarketPriceController()
        {
            marketRepo = new MarketPricesRepository();
        }
        public IActionResult Index()
        {
            var market = marketRepo.GetAll().ToList();
            return View(market);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(MarketPrice market)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    marketRepo.Insert(market);
                    return Redirect("/Administrator/MarketPrice/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(market);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var market = marketRepo.GetById(id);
            if (market == null)
            {
                return NotFound();

            }
            return View(market);
        }
        [HttpPost]
        public IActionResult Edit(MarketPrice market)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    marketRepo.Update(market);
                    return Redirect("/Administrator/MarketPrice/Index");
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return Redirect("/Administrator/MarketPrice/Index");
        }
        public IActionResult Delete(int id)
        {
            marketRepo.Delete(id);
            return Redirect("/Administrator/MarketPrice/Index");
        }
    }
}
