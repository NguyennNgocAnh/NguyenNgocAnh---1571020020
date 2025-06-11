using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Website.Model.Entities;
using Website.Repository;
using Microsoft.EntityFrameworkCore;

public class MarketPricesController : Controller
{
    private readonly MarketPricesRepository marketRepo;
    private readonly NewsRepository newsRepo;
    private readonly ReportRepository reportRepo;
    private readonly StockRepository stockRepo;
    private readonly StockContext _context;
    private static string GetRandomColor()
    {
        var rand = new Random();
        return $"rgba({rand.Next(50, 200)}, {rand.Next(50, 200)}, {rand.Next(50, 200)}, 1)";
    }

    public MarketPricesController()
    {
        marketRepo = new MarketPricesRepository();
        newsRepo = new NewsRepository();
        reportRepo = new ReportRepository();
        stockRepo = new StockRepository();
        _context = new StockContext();
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var today = DateTime.Today;

        // Lấy dữ liệu thực đến ngày hôm nay
        var realMarketData = (from m in marketRepo.GetAll()
                              join s in stockRepo.GetAll() on m.StockId equals s.StockId
                              where m.PriceDate <= today
                              orderby m.PriceDate descending
                              select new
                              {
                                  m.StockId,
                                  s.StockName,
                                  m.PriceDate,
                                  m.ReferencePrice,
                                  m.OpenPrice,
                                  m.CeilingPrice,
                                  m.FloorPrice,
                                  m.HighPrice,
                                  m.LowPrice,
                                  m.AveragePrice,
                                  m.MatchedVolume,
                                  m.MatchedPrice,
                                  m.Buy,
                                  m.Sell,
                                  m.Change
                              }).ToList();

        // Lấy ngày cuối có dữ liệu thực (để tạo dự báo)
        var lastDate = realMarketData.Any() ? realMarketData.Max(m => m.PriceDate) ?? today : today;

        // Tạo dữ liệu dự báo cho 7 ngày kế tiếp từ lastDate
        var forecastData = new List<dynamic>();
        var stocks = stockRepo.GetAll().ToList();

        foreach (var stock in stocks)
        {
            var lastReal = realMarketData
                .Where(m => m.StockId == stock.StockId)
                .OrderByDescending(m => m.PriceDate)
                .FirstOrDefault();

            if (lastReal != null)
            {
                decimal lastPrice = lastReal.MatchedPrice ?? 10m;
                for (int i = 1; i <= 7; i++)
                {
                    var forecastDate = lastDate.AddDays(i);
                    if (forecastDate <= today) continue; // không show dự báo ngày trước hoặc bằng ngày hôm nay

                    var rand = new Random(Guid.NewGuid().GetHashCode());
                    var fluctuation = (decimal)(rand.NextDouble() * 4 - 2); // ±2%
                    var forecastPrice = Math.Round(lastPrice * (1 + fluctuation / 100), 2);

                    forecastData.Add(new
                    {
                        StockId = stock.StockId,
                        StockName = stock.StockName,
                        PriceDate = forecastDate,
                        ReferencePrice = lastPrice,
                        OpenPrice = forecastPrice,
                        CeilingPrice = forecastPrice * 1.07m,
                        FloorPrice = forecastPrice * 0.93m,
                        HighPrice = forecastPrice * 1.02m,
                        LowPrice = forecastPrice * 0.98m,
                        AveragePrice = forecastPrice,
                        MatchedVolume = rand.Next(1000, 10000),
                        MatchedPrice = forecastPrice,
                        Buy = rand.Next(500, 5000),
                        Sell = rand.Next(500, 5000),
                        Change = Math.Round(forecastPrice - lastPrice, 2)
                    });
                }
            }
        }

        // Gộp dữ liệu thực + dự báo (dự báo chỉ ngày tương lai)
        var marketListFull = realMarketData.Concat(forecastData)
                              .OrderByDescending(m => m.PriceDate)
                              .ToList();

        // Tổng số trang
        int totalItems = marketListFull.Count;
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Lấy dữ liệu phân trang
        var marketList = marketListFull.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.MarketList = marketList;

        // --- Biểu đồ 1: Số lượng bản ghi theo tháng ---
        var months = marketListFull
            .Select(m => m.PriceDate?.ToString("yyyy-MM"))
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        var dataset1 = marketListFull
            .GroupBy(m => string.IsNullOrEmpty(m.StockName) ? $"Mã {m.StockId}" : m.StockName)
            .Select(g => new
            {
                label = g.Key,
                data = months.Select(month =>
                    g.Count(x => x.PriceDate?.ToString("yyyy-MM") == month)
                ).ToList(),
                borderColor = GetRandomColor(),
                tension = 0.3,
                fill = false
            }).ToList();

        ViewBag.Labels1 = months;
        ViewBag.Datasets1 = dataset1;

        // --- Biểu đồ 2: Giá cao nhất theo năm ---
        var years = marketListFull
            .Select(m => m.PriceDate?.Year.ToString())
            .Distinct()
            .OrderBy(y => y)
            .ToList();

        var dataset2 = marketListFull
            .GroupBy(m => string.IsNullOrEmpty(m.StockName) ? $"Mã {m.StockId}" : m.StockName)
            .Select(g =>
            {
                var color = GetRandomColor();
                return new
                {
                    label = g.Key,
                    data = years.Select(year =>
                    {
                        var items = g.Where(x => x.PriceDate?.Year.ToString() == year).ToList();
                        if (!items.Any()) return 0;
                        return items.Max(x => x.MatchedPrice ?? 0m);
                    }).ToList(),
                    backgroundColor = color,
                    borderColor = color,
                    borderWidth = 1
                };
            }).ToList();

        ViewBag.Labels2 = years;
        ViewBag.Datasets2 = dataset2;

        // --- Biểu đồ 3: % biến động giá theo tháng ---
        var dataset3 = marketListFull
            .GroupBy(m => string.IsNullOrEmpty(m.StockName) ? $"Mã {m.StockId}" : m.StockName)
            .Select(g =>
            {
                var color = GetRandomColor();
                return new
                {
                    label = g.Key,
                    data = months.Select(month =>
                    {
                        var entries = g.Where(x => x.PriceDate?.ToString("yyyy-MM") == month);
                        if (!entries.Any()) return 0;
                        var avg = entries.Average(x =>
                        {
                            var refPrice = x.ReferencePrice ?? 0m;
                            var matched = x.MatchedPrice ?? 0m;
                            return (decimal)((refPrice == 0m) ? 0m : ((matched - refPrice) / refPrice * 100m));
                        });
                        return Math.Round(avg, 2);
                    }).ToList(),
                    backgroundColor = color,
                    borderColor = color,
                    borderWidth = 1
                };
            }).ToList();

        ViewBag.Labels3 = months;
        ViewBag.Datasets3 = dataset3;

        // --- Biểu đồ 4: Số lượng bản ghi theo năm ---
        var dataset4 = marketListFull
            .GroupBy(m => string.IsNullOrEmpty(m.StockName) ? $"Mã {m.StockId}" : m.StockName)
            .Select(g =>
            {
                var color = GetRandomColor();
                return new
                {
                    label = g.Key,
                    data = years.Select(year =>
                        g.Count(x => x.PriceDate?.Year.ToString() == year)
                    ).ToList(),
                    backgroundColor = color,
                    borderColor = color
                };
            }).ToList();

        ViewBag.Labels4 = years;
        ViewBag.Datasets4 = dataset4;

        // Tin tức liên quan
        var relatedNews = newsRepo.GetAll()
            .Where(n => n.CategoryId == 1)
            .OrderByDescending(n => n.PublishDate)
            .Take(6)
            .ToList();
        ViewBag.RelatedNews = relatedNews;

        // Danh sách cổ phiếu
        var stockList = _context.Stocks
            .Include(s => s.Industry)
            .ToList();

        ViewBag.StockList = stockList;

        // Báo cáo liên quan theo từ khóa
        var keywords = new List<string>
        {
            "CÔNG BỐ THÔNG TIN BẤT THƯỜNG",
            "EXTRAORDINARY INFORMATION DISCLOSURE",
            "NGÂN HÀNG TMCP ĐẦU TƯ",
            "BANK FOR INVESTMENT AND DEVELOPMENT OF VIETNAM",
            "CAP: 21.5.2025, giao dịch 190.681 cổ phiếu ESOP",
            "Ngày giao dịch đầu tiên của cổ phiếu niêm yết bổ sung",
            "ASP: 23.5.2025, ngày GDKHQ tham dự ĐHĐCĐ thường niên năm 2025",
            "Về ngày đăng ký cuối cùng",
        };

        var relatedReports = reportRepo.GetAll()
            .Where(r =>
                (r.Title != null && keywords.Any(k => r.Title.Contains(k, StringComparison.OrdinalIgnoreCase))) ||
                (r.Content != null && keywords.Any(k => r.Content.Contains(k, StringComparison.OrdinalIgnoreCase))))
            .ToList();

        ViewBag.RelatedReports = relatedReports;

        // Thông tin phân trang
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;

        return View();
    }
}
