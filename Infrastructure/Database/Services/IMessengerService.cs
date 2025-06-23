using System.Threading.Tasks;

namespace Autopark.Infrastructure.Database.Services;

public interface IMessengerService
{
    Task SendMessageAsync(string phone, string message);
}

public enum MessengerType
{
    Telegram,
    WhatsApp
}