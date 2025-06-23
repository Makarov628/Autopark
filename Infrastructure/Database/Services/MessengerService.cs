using System;
using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public class MessengerService : IMessengerService
{
    public Task SendMessageAsync(string phone, string message)
    {
        // Заглушка - в реальном проекте здесь будет отправка SMS/сообщений
        Console.WriteLine($"Message would be sent to {phone}: {message}");
        return Task.CompletedTask;
    }

    public Task SendTelegramOtpAsync(string telegramUserId, string otp)
    {
        // Здесь будет интеграция с Telegram API для отправки OTP
        Console.WriteLine($"[Telegram OTP] To: {telegramUserId}, OTP: {otp}");
        return Task.CompletedTask;
    }
}