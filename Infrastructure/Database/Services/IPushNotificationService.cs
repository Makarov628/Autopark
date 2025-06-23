using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public interface IPushNotificationService
{
    Task SendPushNotificationAsync(string deviceToken, string title, string message);
}