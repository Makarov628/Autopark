using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Autopark.Infrastructure.Database.Services;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeZonesController : ControllerBase
{
    private readonly ITimeZoneService _timeZoneService;

    public TimeZonesController(ITimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<IEnumerable<TimeZoneDto>> GetAvailableTimeZones()
    {
        var timeZones = _timeZoneService.GetAvailableTimeZones();
        var dtos = timeZones.Select(tz => new TimeZoneDto(
            Id: tz.Id,
            DisplayName: tz.DisplayName,
            BaseUtcOffset: tz.BaseUtcOffset,
            SupportsDaylightSavingTime: tz.SupportsDaylightSavingTime
        ));
        return Ok(dtos);
    }

    [HttpPost("convert")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<DateTime> ConvertTime(
        [FromBody] TimeZoneConversionRequest request)
    {
        if (!_timeZoneService.IsValidTimeZone(request.EnterpriseTimeZoneId))
            return BadRequest("Неверная таймзона предприятия");

        if (!_timeZoneService.IsValidTimeZone(request.ClientTimeZoneId))
            return BadRequest("Неверная таймзона клиента");

        var convertedTime = _timeZoneService.ConvertFromEnterpriseToClientTime(
            request.EnterpriseTime,
            request.EnterpriseTimeZoneId,
            request.ClientTimeZoneId);

        return Ok(convertedTime);
    }
}

public record TimeZoneDto(
    string Id,
    string DisplayName,
    TimeSpan BaseUtcOffset,
    bool SupportsDaylightSavingTime
);

public record TimeZoneConversionRequest(
    DateTime EnterpriseTime,
    string? EnterpriseTimeZoneId,
    string? ClientTimeZoneId);