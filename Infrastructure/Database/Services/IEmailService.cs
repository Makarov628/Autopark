using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendActivationEmailAsync(string to, string activationToken);
}