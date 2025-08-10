using Autopark.UseCases.Trip.Queries.GetTrips;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TripsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TripsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Получить список поездок транспортного средства за указанный период
    /// </summary>
    /// <param name="vehicleId">ID транспортного средства</param>
    /// <param name="from">Начальная дата и время (в таймзоне предприятия)</param>
    /// <param name="to">Конечная дата и время (в таймзоне предприятия)</param>
    /// <param name="tz">Таймзона предприятия (по умолчанию UTC)</param>
    /// <returns>Список поездок с информацией о начальных и конечных точках</returns>
    [HttpGet("{vehicleId}")]
    public async Task<IActionResult> GetTrips(
        int vehicleId,
        DateTime from,
        DateTime to,
        string tz = "UTC")
    {
        try
        {
            var query = new GetTripsQuery(vehicleId, from, to, tz);
            var response = await _mediator.Send(query);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving trips: {ex.Message}");
        }
    }

    /// <summary>
    /// Получить трек транспортного средства за указанный период с поездками
    /// </summary>
    /// <param name="vehicleId">ID транспортного средства</param>
    /// <param name="from">Начальная дата и время</param>
    /// <param name="to">Конечная дата и время</param>
    /// <param name="tz">Таймзона (по умолчанию UTC)</param>
    /// <param name="format">Формат ответа (json или gpx)</param>
    /// <returns>Трек с поездками и точками трека</returns>
    [HttpGet("{vehicleId}/track")]
    public async Task<IActionResult> GetTrack(
        int vehicleId,
        DateTime from,
        DateTime to,
        string tz = "UTC",
        string format = "json")
    {
        try
        {
            var query = new GetTripsQuery(vehicleId, from, to, tz);
            var response = await _mediator.Send(query);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving track: {ex.Message}");
        }
    }

    /// <summary>
    /// Получить информацию о конкретной поездке
    /// </summary>
    /// <param name="tripId">ID поездки</param>
    /// <returns>Детальная информация о поездке</returns>
    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetTrip(long tripId)
    {
        try
        {
            // Здесь можно добавить отдельный UseCase для получения одной поездки
            // Пока возвращаем заглушку
            return Ok(new { Message = "Trip details endpoint - to be implemented", TripId = tripId });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving trip: {ex.Message}");
        }
    }
}