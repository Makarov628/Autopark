using Microsoft.AspNetCore.Mvc;
using Octonica.ClickHouseClient;
using TimeZoneConverter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed record TrackPoint(
    DateTime LocalTime,
    double Latitude,
    double Longitude,
    float Speed,
    ushort Rpm,
    byte FuelLevel);

[ApiController]
[Route("api/[controller]")]
public sealed class TracksController : ControllerBase
{
    private readonly ClickHouseConnection _conn;

    public TracksController(ClickHouseConnection conn) => _conn = conn;

    [HttpGet("{vehicleId}")]
    public async Task<IActionResult> Get(
        uint vehicleId,
        DateTime from,
        DateTime to,
        string tz = "UTC",
        string format = "json")
    {
        const string sql = @"
            SELECT ts, pos.2 AS lat, pos.1 AS lon,
                   speed, rpm, fuel_lvl
            FROM   can_telemetry
            WHERE  vehicle_id = {vid:UInt32}
              AND  ts BETWEEN {from:DateTime64} AND {to:DateTime64}
            ORDER BY ts";

        await _conn.OpenAsync();
        await using var cmd = _conn.CreateCommand(sql);
        cmd.Parameters.AddWithValue("vid", vehicleId);
        cmd.Parameters.AddWithValue("from", from);
        cmd.Parameters.AddWithValue("to", to);

        var tzInfo = TZConvert.GetTimeZoneInfo(tz);
        var list = new List<TrackPoint>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var utc = reader.GetDateTime(0);
            var loc = TimeZoneInfo.ConvertTimeFromUtc(utc, tzInfo);
            list.Add(new TrackPoint(
                loc,
                reader.GetDouble(1),
                reader.GetDouble(2),
                reader.GetFloat(3),
                reader.GetUInt16(4),
                reader.GetByte(5)
            ));
        }

        return format.Equals("geojson", StringComparison.OrdinalIgnoreCase)
            ? Ok(GeoJsonHelper.ToFeatureCollection(list))
            : Ok(list);
    }
}