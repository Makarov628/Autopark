using System;
using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public class PushNotificationService : IPushNotificationService
{
    public Task SendPushNotificationAsync(string deviceToken, string title, string message)
    {
        // Заглушка - в реальном проекте здесь будет отправка push уведомлений
        Console.WriteLine($"Push notification would be sent to {deviceToken}: {title} - {message}");
        return Task.CompletedTask;
    }
}