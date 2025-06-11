using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Website.Model.Entities;

[ApiController]
[Route("api/[controller]")]
public class CrawlController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly StockContext _context;

    public CrawlController(HttpClient httpClient, StockContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    [HttpGet("preview-stock-data")]
    public async Task<IActionResult> PreviewStockData()
    {
        try
        {
            var url = "https://apipubaws.tcbs.com.vn/stock-insight/v2/stock/bars-long-term?ticker=ACB&type=stock&resolxdđsution=D&to=1744986338&countBack=365";
            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);
            var data = json["data"] as JArray;

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Không thể lấy dữ liệu", detail = ex.Message });
        }
    }


    [HttpPost("crawl-and-save-multiple")]
    public async Task<IActionResult> CrawlAndSaveMultiple([FromBody] List<string> tickers)
    {
        try
        {
            if (tickers == null || tickers.Count == 0)
            {
                return BadRequest(new { message = "Danh sách mã chứng khoán trống." });
            }

            var to = "1744986338";
            var countBack = "365";

            foreach (var ticker in tickers)
            {
                var url = $"https://apipubaws.tcbs.com.vn/stock-insight/v2/stock/bars-long-term?ticker={ticker}&type=stock&resolution=D&to={to}&countBack={countBack}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                var dataList = json["data"] as JArray;
                var stock = _context.Stocks.FirstOrDefault(x => x.StockName == ticker);

                if (stock == null)
                {
                    continue; // bỏ qua nếu không tìm thấy mã
                }

                foreach (var item in dataList)
                {
                    // Parse ngày cho an toàn
                    DateTime? tradingDate = null;
                    if (DateTime.TryParse(item["tradingDate"]?.ToString(), out var parsedDate))
                        tradingDate = parsedDate;

                    var marketPrice = new MarketPrice
                    {
                        // KHÔNG gán PriceId nếu là Identity (tự tăng)
                        PriceDate = tradingDate,
                        OpenPrice = TryParseDecimal(item["open"]),
                        LowPrice = TryParseDecimal(item["close"]),
                        CeilingPrice = TryParseDecimal(item["high"]) ?? 0,
                        FloorPrice = TryParseDecimal(item["low"]) ?? 0,
                        MatchedVolume = item["volume"]?.ToObject<int>(),
                        StockId = stock.StockId,

                        ReferencePrice = null,
                        HighPrice = null,
                        MatchedPrice = null,
                        AveragePrice = null,
                        Buy = null,
                        Sell = null,
                        Change = null,
                    };

                    _context.MarketPrices.Add(marketPrice);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã lưu dữ liệu cho nhiều mã chứng khoán." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "Lỗi khi crawl dữ liệu",
                detail = ex.InnerException?.Message ?? ex.Message // thêm InnerException
            });
        }
    }

    // Hàm hỗ trợ chuyển đổi an toàn decimal
    private decimal? TryParseDecimal(JToken token)
    {
        if (token == null) return null;
        if (decimal.TryParse(token.ToString(), out var result))
            return result;
        return null;
    }
}