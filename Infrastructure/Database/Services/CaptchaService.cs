using System;
using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public class CaptchaService : ICaptchaService
{
    public Task<bool> VerifyCaptchaAsync(string captchaToken)
    {
        // Заглушка - в реальном проекте здесь будет проверка captcha
        Console.WriteLine($"Captcha verification for token: {captchaToken}");
        return Task.FromResult(true);
    }
}