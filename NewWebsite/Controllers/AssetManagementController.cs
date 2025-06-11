using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewWebsite.Filters;
using Website.Model.Entities;
using Website.Repository;

namespace NewWebsite.Controllers
{
    [SessionAuthorize]
    public class AssetManagementController : Controller
    {
        private readonly AssetManagementRepository assetRepo;
        private readonly ExportRepository exportRepo;
        private readonly StockRepository stockRepo;
        private readonly PriceHistoryRepository priceHistoryRepo;
        private readonly MarketPricesRepository marketPricesRepo;
        private readonly StockContext _context;

        public AssetManagementController()
        {
            assetRepo = new AssetManagementRepository();
            exportRepo = new ExportRepository();
            stockRepo = new StockRepository();
            priceHistoryRepo = new PriceHistoryRepository();
            marketPricesRepo = new MarketPricesRepository();
            _context = new StockContext();
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate, string searchKeyword = "")
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy tất cả tài sản của người dùng
            var allAssets = assetRepo.GetAll()
                                   .Where(a => a.UserId == userId.Value)
                                   .ToList();

            // Gộp các tài sản trùng nhau (cùng AssetCode)
            var groupedAssets = allAssets
                .GroupBy(a => new { a.AssetCode, a.UserId })
                .Select(g => new AssetManagement
                {
                    AssetId = g.First().AssetId,
                    AssetCode = g.Key.AssetCode,
                    UserId = g.Key.UserId,
                    Quantity = g.Sum(a => a.Quantity),
                    BuyPrice = Math.Round(g.Sum(a => a.Quantity * a.BuyPrice) / g.Sum(a => a.Quantity), 2),
                    SellPrice = g.Max(a => a.SellPrice) ?? 0, // Gán 0 nếu không có SellPrice
                    ActionDate = g.Max(a => a.ActionDate),
                    AssetName = g.First().AssetName,
                    StockId = g.First().StockId,
                    Action = g.First().Action
                })
                .ToList();

            // Filter theo điều kiện
            var assets = groupedAssets.AsQueryable();
            if (startDate.HasValue)
                assets = assets.Where(p => p.ActionDate.HasValue && p.ActionDate.Value.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                assets = assets.Where(p => p.ActionDate.HasValue && p.ActionDate.Value.Date <= endDate.Value.Date);
            if (!string.IsNullOrEmpty(searchKeyword))
                assets = assets.Where(p => p.AssetCode.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase));

            // Lấy giá thị trường gần nhất để cập nhật SellPrice
            foreach (var asset in assets)
            {
                var latestPrice = marketPricesRepo.GetAll()
                    .Where(mp => mp.StockId == asset.StockId)
                    .OrderByDescending(mp => mp.PriceDate)
                    .FirstOrDefault();
                if (latestPrice != null && latestPrice.MatchedPrice.HasValue)
                {
                    asset.SellPrice = Math.Round(latestPrice.MatchedPrice.Value, 2);
                }
                else
                {
                    asset.SellPrice = asset.SellPrice ?? 0; // Gán 0 nếu không có giá từ thị trường
                }

                // Tính lãi/lỗ dựa trên giá bán từ thị trường, giới hạn 2 số thập phân
                asset.ProfitLoss = Math.Round((asset.SellPrice.Value - asset.BuyPrice) * asset.Quantity, 2); // Sử dụng Value để ép kiểu
            }

            // Tính tổng lãi/lỗ hiện tại, giới hạn 2 số thập phân
            decimal? totalProfitLossNow = assets.Sum(a => a.ProfitLoss);

            // DỰ ĐOÁN LÃI/LỖ TƯƠNG LAI
            decimal totalProfitLossFuture = 0;
            foreach (var asset in assets)
            {
                decimal predictedSellPrice = Math.Round(asset.BuyPrice * 1.05m, 2); // Tăng 5%, giới hạn 2 số
                decimal predictedProfitLoss = Math.Round((predictedSellPrice - asset.BuyPrice) * asset.Quantity, 2);
                totalProfitLossFuture += predictedProfitLoss;
            }

            // Truyền dữ liệu ra View bằng ViewBag với 2 số thập phân
            ViewBag.TotalProfitLossNow = totalProfitLossNow?.ToString("F2");
            ViewBag.TotalProfitLossFuture = totalProfitLossFuture.ToString("F2");

            // Lấy danh sách cổ phiếu
            var stockList = _context.Stocks
                .Include(s => s.Industry)
                .ToList();
            ViewBag.StockList = stockList;

            // Chuẩn bị dữ liệu cho biểu đồ tỉ lệ giao dịch
            var transactionsByDate = assets
                .Where(a => a.ActionDate.HasValue)
                .GroupBy(t => t.ActionDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalQuantity = g.Sum(t => t.Quantity)
                })
                .OrderBy(g => g.Date)
                .ToList();

            ViewBag.TransactionDates = transactionsByDate.Select(t => t.Date.ToString("dd/MM/yyyy")).ToList();
            ViewBag.TransactionQuantities = transactionsByDate.Select(t => t.TotalQuantity).ToList();

            return View(assets.ToList());
        }

        [HttpGet]
        public IActionResult Create(string stockName)
        {
            var model = new AssetManagement();
            if (!string.IsNullOrEmpty(stockName))
            {
                model.AssetCode = stockName;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(AssetManagement asset)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    if (userId != null)
                    {
                        asset.UserId = userId.Value;
                    }

                    // Kiểm tra xem đã có tài sản trùng AssetCode và Action "Mua" chưa
                    var existingAsset = assetRepo.GetAll().FirstOrDefault(a =>
                        a.UserId == asset.UserId &&
                        a.AssetCode.Equals(asset.AssetCode, StringComparison.OrdinalIgnoreCase) &&
                        a.Action.Equals("Mua", StringComparison.OrdinalIgnoreCase)
                    );

                    if (existingAsset != null)
                    {
                        int totalQuantity = existingAsset.Quantity + asset.Quantity;
                        decimal totalBuyValue = (existingAsset.BuyPrice * existingAsset.Quantity) + (asset.BuyPrice * asset.Quantity);
                        decimal newAvgBuyPrice = Math.Round(totalBuyValue / totalQuantity, 2);

                        existingAsset.Quantity = totalQuantity;
                        existingAsset.BuyPrice = newAvgBuyPrice;
                        existingAsset.ActionDate = asset.ActionDate;
                        existingAsset.SellPrice = asset.SellPrice.HasValue ? Math.Round(asset.SellPrice.Value, 2) : 0; // Xử lý null

                        assetRepo.Update(existingAsset);
                    }
                    else
                    {
                        asset.BuyPrice = Math.Round(asset.BuyPrice, 2);
                        asset.SellPrice = asset.SellPrice.HasValue ? Math.Round(asset.SellPrice.Value, 2) : 0; // Xử lý null
                        assetRepo.Insert(asset);
                    }

                    if (asset.StockId > 0 && asset.ActionDate != null)
                    {
                        var priceHistory = new PriceHistory
                        {
                            StockId = asset.StockId,
                            Date = asset.ActionDate,
                            Price = Math.Round(asset.BuyPrice, 2),
                            ClosePrice = null,
                            MatchPrice = null,
                            OpenPrice = null,
                            HighPrice = null,
                            LowPrice = null
                        };

                        priceHistoryRepo.Insert(priceHistory);
                    }
                    TempData["Success"] = "Tạo tài sản thành công!";
                    return RedirectToAction("Index");
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                asset.UserId = userId.Value;
            }

            if (ModelState.IsValid)
            {
                asset.BuyPrice = Math.Round(asset.BuyPrice, 2);
                asset.SellPrice = asset.SellPrice.HasValue ? Math.Round(asset.SellPrice.Value, 2) : 0; // Xử lý null
                asset.ProfitLoss = Math.Round(((asset.SellPrice ?? 0) - asset.BuyPrice) * asset.Quantity, 2); // Tính lại ProfitLoss
                assetRepo.Update(asset);
                return RedirectToAction("Index");
            }
            return View(asset);
        }

        public IActionResult ExportToExcel()
        {
            var assets = assetRepo.GetAll().ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Báo cáo tài sản");

            // Tiêu đề các cột
            worksheet.Cell(1, 1).Value = "Mã tài sản";
            worksheet.Cell(1, 2).Value = "Tên tài sản";
            worksheet.Cell(1, 3).Value = "Hành động";
            worksheet.Cell(1, 4).Value = "Số lượng";
            worksheet.Cell(1, 5).Value = "Giá mua";
            worksheet.Cell(1, 6).Value = "Giá bán";
            worksheet.Cell(1, 7).Value = "Lãi/Lỗ";
            worksheet.Cell(1, 8).Value = "Ngày thực hiện";

            // Ghi dữ liệu từ danh sách, giới hạn 2 số thập phân
            int row = 2;
            foreach (var asset in assets)
            {
                worksheet.Cell(row, 1).Value = asset.AssetCode ?? "";
                worksheet.Cell(row, 2).Value = asset.AssetName ?? "";
                worksheet.Cell(row, 3).Value = asset.Action ?? "";
                worksheet.Cell(row, 4).Value = asset.Quantity;
                worksheet.Cell(row, 5).Value = Math.Round(asset.BuyPrice, 2);
                worksheet.Cell(row, 6).Value = Math.Round(asset.SellPrice ?? 0, 2);
                worksheet.Cell(row, 7).Value = Math.Round(asset.ProfitLoss ?? 0, 2);
                worksheet.Cell(row, 8).Value = asset.ActionDate?.ToString("dd/MM/yyyy") ?? "";
                row++;
            }

            // Trả về file Excel
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "BaoCao_TaiSan.xlsx");
        }

        public IActionResult Delete(int id)
        {
            assetRepo.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult GetMarketPrices(int stockId)
        {
            var marketList = _context.MarketPrices
                                     .Where(m => m.StockId == stockId)
                                     .OrderByDescending(m => m.PriceDate)
                                     .ToList();

            return PartialView("PriceDetails", marketList);
        }

        [HttpPost]
        public IActionResult History(DateTime? startDate, DateTime? endDate, string searchKeyword = "")
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return PartialView("_HistoryTable", new List<AssetManagement>());
            }

            var assetHistories = assetRepo.GetAll()
                .Where(a => a.UserId == userId.Value)
                .ToList();

            if (startDate.HasValue)
                assetHistories = assetHistories.Where(p => p.ActionDate.HasValue && p.ActionDate.Value.Date >= startDate.Value.Date).ToList();
            if (endDate.HasValue)
                assetHistories = assetHistories.Where(p => p.ActionDate.HasValue && p.ActionDate.Value.Date <= endDate.Value.Date).ToList();
            if (!string.IsNullOrEmpty(searchKeyword))
                assetHistories = assetHistories.Where(p => p.AssetCode.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase)).ToList();

            var result = assetHistories
                .OrderByDescending(a => a.ActionDate)
                .ToList();

            return PartialView("_HistoryTable", result);
        }
    }
}