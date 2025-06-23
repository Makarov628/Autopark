using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public class EmailService : IEmailService
{
    public EmailService()
    {
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        // Заглушка - в реальном проекте здесь будет отправка email
        Console.WriteLine($"Email would be sent to {to}: {subject}");
        return Task.CompletedTask;
    }

    public Task SendActivationEmailAsync(string to, string activationToken)
    {
        var subject = "Активация учетной записи";
        var body = $"Для активации вашей учетной записи перейдите по ссылке: /activate?token={activationToken}";
        return SendEmailAsync(to, subject, body);
    }
}