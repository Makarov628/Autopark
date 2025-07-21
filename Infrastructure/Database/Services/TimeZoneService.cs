using System.Globalization;

namespace Autopark.Infrastructure.Database.Services;

public class TimeZoneService : ITimeZoneService
{
    /// <summary>
    /// Конвертирует UTC время в время таймзоны предприятия
    /// Если таймзона не задана - возвращает UTC
    /// </summary>
    public DateTime ConvertFromUtcToEnterpriseTime(DateTime utcTime, string? enterpriseTimeZoneId)
    {
        if (string.IsNullOrEmpty(enterpriseTimeZoneId))
            return utcTime; // Если таймзона не задана, возвращаем UTC

        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(enterpriseTimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return utcTime; // Если таймзона не найдена, возвращаем UTC
        }
    }

    /// <summary>
    /// Конвертирует время таймзоны предприятия в UTC
    /// Если таймзона не задана - считает время уже в UTC
    /// </summary>
    public DateTime ConvertFromEnterpriseTimeToUtc(DateTime enterpriseTime, string? enterpriseTimeZoneId)
    {
        if (string.IsNullOrEmpty(enterpriseTimeZoneId))
            return enterpriseTime; // Если таймзона не задана, считаем что время уже в UTC

        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(enterpriseTimeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(enterpriseTime, timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return enterpriseTime; // Если таймзона не найдена, считаем что время уже в UTC
        }
    }

    /// <summary>
    /// Конвертирует время из таймзоны предприятия в таймзону клиента
    /// </summary>
    public DateTime ConvertFromEnterpriseToClientTime(DateTime enterpriseTime, string? enterpriseTimeZoneId, string? clientTimeZoneId)
    {
        // Сначала конвертируем время предприятия в UTC
        var utcTime = ConvertFromEnterpriseTimeToUtc(enterpriseTime, enterpriseTimeZoneId);

        // Затем конвертируем из UTC в таймзону клиента
        if (string.IsNullOrEmpty(clientTimeZoneId))
            return utcTime; // Если таймзона клиента не задана, возвращаем UTC

        try
        {
            var clientTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clientTimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, clientTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return utcTime; // Если таймзона клиента не найдена, возвращаем UTC
        }
    }

    /// <summary>
    /// Конвертирует UTC DateTimeOffset в время таймзоны предприятия
    /// Возвращает DateTimeOffset с правильным смещением
    /// </summary>
    public DateTimeOffset ToEnterpriseZone(DateTimeOffset utc, string? tzId)
    {
        if (string.IsNullOrEmpty(tzId))
            return utc; // Если таймзона не задана, возвращаем UTC

        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utc.UtcDateTime, timeZone);
            return new DateTimeOffset(localTime, timeZone.GetUtcOffset(localTime));
        }
        catch (TimeZoneNotFoundException)
        {
            return utc; // Если таймзона не найдена, возвращаем UTC
        }
    }

    /// <summary>
    /// Получает список доступных таймзон, отсортированных по displayName
    /// </summary>
    public IEnumerable<TimeZoneInfo> GetAvailableTimeZones()
    {
        return TimeZoneInfo.GetSystemTimeZones()
            .OrderBy(tz => tz.DisplayName);
    }

    /// <summary>
    /// Проверяет, является ли таймзона валидной
    /// </summary>
    public bool IsValidTimeZone(string? timeZoneId)
    {
        if (string.IsNullOrEmpty(timeZoneId))
            return true; // Пустая таймзона считается валидной (будет использоваться UTC)

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
    }
}