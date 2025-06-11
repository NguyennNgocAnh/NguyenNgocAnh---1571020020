using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Website.Repository;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpClient = new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Smtp:Username"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"Email sent successfully to {toEmail}");
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"SMTP Error: {ex.Message}, StatusCode: {ex.StatusCode}");
            throw; // Ném lại lỗi để xử lý ở tầng trên
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error sending email to {toEmail}: {ex.Message}");
            throw;
        }
    }
}
