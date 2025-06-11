using Microsoft.AspNetCore.Mvc;
using Website.Model.Entities;
using Website.Repository;

namespace NewWebsite.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class AssetManagementController : Controller
    {
        private AssetManagementRepository assetRepo;
        public AssetManagementController()
        {
            assetRepo = new AssetManagementRepository();
        }
        public IActionResult Index()
        {
            var asset = assetRepo.GetAll().ToList();
            return View(asset);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(AssetManagement asset)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    assetRepo.Insert(asset);
                    return Redirect("/Administrator/AssetManagement/Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(asset);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var asset = assetRepo.GetById(id);
            if (asset == null)
            {
                return NotFound();

            }
            return View(asset);
        }
        [HttpPost]
        public IActionResult Edit(AssetManagement asset)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    assetRepo.Update(asset);
                    return Redirect("/Administrator/AssetManagement/Index");
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }
            return Redirect("/Administrator/AssetManagement/Index");
        }
        public IActionResult Delete(int id)
        {
            assetRepo.Delete(id);
            return Redirect("/Administrator/AssetManagement/Index");
        }
    }
}
