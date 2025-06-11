using Microsoft.AspNetCore.Mvc;
using Website.Repository;

namespace NewWebsite.Controllers
{
    public class ForecastController : Controller
    {
        private readonly ForecastServiceRepository _forecastService;
        private readonly StockRepository stockRepo;

        public ForecastController(ForecastServiceRepository forecastService)
        {
            _forecastService = forecastService;
            stockRepo = new StockRepository();
        }

        // Hàm tiện ích lấy danh sách cổ phiếu gán vào ViewBag
        private void LoadStockList()
        {
            ViewBag.StockList = stockRepo.GetAll().ToList();

        }

        public IActionResult Index()
        {
            LoadStockList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AutoPredict(int stockId = 1)
        {
            try
            {
                var priceRepo = new MarketPricesRepository();
                var prices = priceRepo.GetAll()
                                .Where(m => m.StockId == stockId && m.MatchedPrice.HasValue)
                                .OrderByDescending(m => m.PriceDate)
                                .Take(7)
                                .Select(m => (float)m.MatchedPrice.Value)
                                .Reverse()
                                .ToArray();

                if (prices.Length < 3)
                {
                    ViewBag.Error = "Không đủ dữ liệu để dự báo.";
                    LoadStockList();  // Bổ sung danh sách cổ phiếu trước khi trả view
                    return View("Index");
                }

                ViewBag.InputValues = string.Join(", ", prices.Select(p => p.ToString("F2")));

                var prediction = await _forecastService.GetPredictionAsync(prices);
                if (prediction == null)
                {
                    ViewBag.Error = "Không thể dự báo. Kiểm tra lại kết nối API.";
                }
                else
                {
                    ViewBag.Prediction = prediction.Value.ToString("F2");
                }
                var selectedStock = stockRepo.GetById(stockId);
                if (selectedStock != null)
                {
                    ViewBag.SelectedStockName = selectedStock.StockName;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi lấy dữ liệu tự động: " + ex.Message;
            }

            LoadStockList();  // Bổ sung danh sách cổ phiếu trước khi trả view
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Index(string inputValues)
        {
            try
            {
                ViewBag.InputValues = inputValues;

                var data = inputValues
                    .Split(',')
                    .Select(x => float.Parse(x.Trim(), System.Globalization.CultureInfo.InvariantCulture))
                    .ToArray();

                var prediction = await _forecastService.GetPredictionAsync(data);

                if (prediction == null)
                {
                    ViewBag.Error = "Không thể dự báo. Kiểm tra lại dữ liệu hoặc kết nối.";
                }
                else
                {
                    ViewBag.Prediction = prediction.Value.ToString("F2");
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi xử lý dữ liệu: " + ex.Message;
            }

            LoadStockList();  // Bổ sung danh sách cổ phiếu trước khi trả view
            return View();
        }

        // (Tuỳ chọn) Dùng nếu bạn cần gọi API từ JS
        [HttpPost]
        public async Task<IActionResult> PredictJson([FromBody] float[] inputData)
        {
            var prediction = await _forecastService.GetPredictionAsync(inputData);

            if (prediction == null)
                return BadRequest("Dự đoán thất bại");

            return Json(new { prediction });
        }
    }
}
