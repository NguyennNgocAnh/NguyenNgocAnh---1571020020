using Microsoft.EntityFrameworkCore;
using Website.Model.Entities;
using Website.Repository;

public class AlertCheckService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public AlertCheckService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StockContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var alerts = await context.Alerts
                    .Include(a => a.Stock)
                    .ThenInclude(s => s.MarketPrices)
                    .Include(a => a.Customer)
                    .ToListAsync();

                foreach (var alert in alerts)
                {

                    if (alert == null || alert.Stock == null || !alert.TriggeredTime.HasValue)
                    {
                        Console.WriteLine($"Lỗi: Alert, Stock hoặc TriggeredTime là null (AlertId: {alert?.AlertId}).");
                        continue;
                    }
                    var marketPrice = alert.Stock.MarketPrices
                        ?.Where(mp => mp.PriceDate.HasValue && mp.PriceDate.Value.Date <= DateTime.Today && mp.OpenPrice.HasValue)
                        .OrderByDescending(mp => mp.PriceDate)
                        .FirstOrDefault();

                    if (marketPrice == null)
                    {
                        Console.WriteLine($"Không tìm thấy giá thị trường hợp lệ cho cổ phiếu {alert.Stock.StockName}.");
                        continue;
                    }

                    if (!marketPrice.OpenPrice.HasValue)
                    {
                        Console.WriteLine($"❌ Không có OpenPrice cho cổ phiếu {alert.Stock.StockName} vào ngày {marketPrice.PriceDate?.ToShortDateString()}");
                        continue;
                    }

                    decimal giaHienTai = marketPrice.OpenPrice.Value;

                    Console.WriteLine($"🧪 Debug: OpenPrice = {giaHienTai}, Ngưỡng = {alert.PriceThresholdUp}, IsTriggered = {alert.IsTriggered}");


                    if (alert.IsTriggered == null)
                    {
                        Console.WriteLine($"Lỗi: IsTriggered của cảnh báo {alert.AlertId} là null.");
                        continue;
                    }
                    if (alert.Stock.MarketPrices == null || !alert.Stock.MarketPrices.Any())
                    {
                        Console.WriteLine($"❌ Không có dữ liệu MarketPrices cho mã {alert.Stock.StockName}");
                    }

                    Console.WriteLine($"Debug: giá hiện tại = {giaHienTai}, ngưỡng cảnh báo = {alert.PriceThresholdUp}, IsTriggered = {alert.IsTriggered}");

                    if (giaHienTai >= alert.PriceThresholdUp.Value && !alert.IsTriggered.Value)
                    {
                        // Lấy User dựa trên CustomerId (giả sử CustomerId = UserId)
                        var user = await context.Users
                            .FirstOrDefaultAsync(u => u.UserId == alert.CustomerId);

                        Console.WriteLine($"🔍 Alert.CustomerId = {alert.CustomerId}, Email = {user?.Email}");

                        if (user == null || string.IsNullOrWhiteSpace(user.Email))
                        {
                            Console.WriteLine($"Lỗi: Không tìm thấy User hoặc email không hợp lệ cho CustomerId {alert.CustomerId}.");
                            continue;
                        }

                        string subject = $"📈 Cảnh báo: Giá cổ phiếu {alert.Stock.StockName} đã vượt ngưỡng!";
                        string body = $"<h3>Chào {user.Username ?? "nhà đầu tư"},</h3>" +
                                      $"<p>Giá cổ phiếu <strong>{alert.Stock.StockName}</strong> đã đạt <strong>{giaHienTai} VND</strong>.</p>" +
                                      $"<p>Ngưỡng bạn thiết lập: <strong>{alert.PriceThresholdUp} VND</strong>.</p>" +
                                      $"<p>Thời gian: <strong>{marketPrice.PriceDate}</strong>.</p>" +
                                      $"<p>Chúng tôi sẽ tiếp tục theo dõi và cập nhật nếu có thêm biến động.</p>" +
                                      $"<p>Trân trọng,<br>Hệ thống cảnh báo giá</p>";

                        try
                        {
                            await emailService.SendEmailAsync(user.Email, subject, body);
                            Console.WriteLine($"✅ Email đã gửi đến {user.Email} at {DateTime.Now}");
                            alert.IsTriggered = true;
                            alert.TriggeredTime = DateTime.Now;
                            alert.TriggerMessage = $"Đã gửi cảnh báo giá {giaHienTai} vượt ngưỡng {alert.PriceThresholdUp}";
                            await context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi gửi email tới {user.Email} at {DateTime.Now}: {ex.Message}");
                        }
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Kiểm tra mỗi 30 giây
        }
    }
}