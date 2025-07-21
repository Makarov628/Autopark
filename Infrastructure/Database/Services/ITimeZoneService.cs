namespace Autopark.Infrastructure.Database.Services;

public interface ITimeZoneService
{
    /// <summary>
    /// Конвертирует UTC время в время таймзоны предприятия
    /// </summary>
    DateTime ConvertFromUtcToEnterpriseTime(DateTime utcTime, string? enterpriseTimeZoneId);

    /// <summary>
    /// Конвертирует время таймзоны предприятия в UTC
    /// </summary>
    DateTime ConvertFromEnterpriseTimeToUtc(DateTime enterpriseTime, string? enterpriseTimeZoneId);

    /// <summary>
    /// Конвертирует время из таймзоны предприятия в таймзону клиента
    /// </summary>
    DateTime ConvertFromEnterpriseToClientTime(DateTime enterpriseTime, string? enterpriseTimeZoneId, string? clientTimeZoneId);

    /// <summary>
    /// Конвертирует UTC DateTimeOffset в время таймзоны предприятия
    /// Возвращает DateTimeOffset с правильным смещением в ISO-8601
    /// </summary>
    DateTimeOffset ToEnterpriseZone(DateTimeOffset utc, string? tzId);

    /// <summary>
    /// Получает список доступных таймзон
    /// </summary>
    IEnumerable<TimeZoneInfo> GetAvailableTimeZones();

    /// <summary>
    /// Проверяет, является ли таймзона валидной
    /// </summary>
    bool IsValidTimeZone(string? timeZoneId);
}