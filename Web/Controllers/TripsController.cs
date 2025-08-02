using Microsoft.AspNetCore.Mvc;
using MediatR;
using Autopark.UseCases.Trip.Queries.GetTripsByDateRange;
using System.Text.Json;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TripsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TripsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{vehicleId}/track")]
    public async Task<IActionResult> GetTrackByTrips(
        int vehicleId,
        DateTime from,
        DateTime to,
        string tz = "UTC",
        string format = "json")
    {
        var query = new GetTripsByDateRangeQuery(vehicleId, from, to, tz);
        var response = await _mediator.Send(query);

        if (format.Equals("geojson", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(ToGeoJson(response.TrackPoints));
        }

        return Ok(response);
    }

    private static object ToGeoJson(Autopark.UseCases.Trip.Queries.GetTripsByDateRange.TrackPoint[] points)
    {
        var coords = points.Select(p => new[] { p.Longitude, p.Latitude });
        return new
        {
            type = "FeatureCollection",
            features = new[]
            {
                new
                {
                    type = "Feature",
                    geometry = new
                    {
                        type = "LineString",
                        coordinates = coords
                    },
                    properties = new { }
                }
            }
        };
    }
}