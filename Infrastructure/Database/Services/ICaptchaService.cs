using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public interface ICaptchaService
{
    Task<bool> VerifyCaptchaAsync(string captchaToken);
}