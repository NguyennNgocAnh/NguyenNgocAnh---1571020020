using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Model.Entities;

namespace NewWebsite.Controllers
{
    public class StockController : Controller
    {
        private readonly StockContext _context;

        public StockController(StockContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var stocks = _context.Stocks.ToList();
            return View(stocks);  
        }

        public IActionResult StockDetail(int id)
        {
            var stock = _context.Stocks
                .Include(s => s.MarketPrices)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null) return NotFound();

            return View(stock); 
        }
        public IActionResult Exchange()
        {
            var stocks = _context.Stocks.ToList();
            return View(stocks);
        }
        public async Task<IActionResult> Details(int id)
        {
            var stock = await _context.Stocks
                .Include(s => s.MarketPrices.OrderByDescending(m => m.PriceDate))
                .FirstOrDefaultAsync(s => s.StockId == id);

            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }
    }
}
