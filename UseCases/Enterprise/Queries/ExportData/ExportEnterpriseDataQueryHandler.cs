using System.Globalization;
using System.Text;
using System.Text.Json;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Enterprise.ValueObjects;

namespace Autopark.UseCases.Enterprise.Queries.ExportData;

public class ExportEnterpriseDataQueryHandler : IRequestHandler<ExportEnterpriseDataQuery, Fin<ExportEnterpriseDataResponse>>
{
    private readonly AutoparkDbContext _context;

    public ExportEnterpriseDataQueryHandler(AutoparkDbContext context)
    {
        _context = context;
    }

    public async Task<Fin<ExportEnterpriseDataResponse>> Handle(ExportEnterpriseDataQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var enterpriseId = EnterpriseId.Create(request.EnterpriseId);

            // Получаем данные предприятия
            var enterprise = await _context.Enterprises
                .Include(e => e.Vehicles)
                    .ThenInclude(v => v.BrandModel)
                .Include(e => e.Drivers)
                    .ThenInclude(d => d.User)
                .Where(e => e.Id == enterpriseId)
                .FirstOrDefaultAsync(cancellationToken);

            if (enterprise == null)
            {
                return Error.New($"Enterprise with id {request.EnterpriseId} not found");
            }

            // Получаем поездки в диапазоне дат
            var tripsQuery = _context.Trips
                .Include(t => t.StartPoint)
                .Include(t => t.EndPoint)
                .Where(t => enterprise.Vehicles.Select(v => v.Id).Contains(t.VehicleId));

            if (request.StartDate.HasValue)
            {
                tripsQuery = tripsQuery.Where(t => t.StartUtc >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                tripsQuery = tripsQuery.Where(t => t.EndUtc <= request.EndDate.Value);
            }

            var tripsList = await tripsQuery.ToListAsync(cancellationToken);

            // Получаем точки треков для поездок
            var trackPointsList = await _context.VehicleTrackPoints
                .Where(tp => enterprise.Vehicles.Select(v => v.Id).Contains(tp.VehicleId) &&
                           tp.TimestampUtc >= (request.StartDate ?? DateTime.MinValue) &&
                           tp.TimestampUtc <= (request.EndDate ?? DateTime.MaxValue))
                .OrderBy(tp => tp.TimestampUtc)
                .ToListAsync(cancellationToken);

            // Создаем объект для экспорта
            var exportData = new ExportData
            {
                Enterprise = new ExportEnterprise
                {
                    Id = enterprise.Id.Value,
                    Name = enterprise.Name.Value,
                    Address = enterprise.Address,
                    TimeZoneId = enterprise.TimeZoneId
                },
                Vehicles = enterprise.Vehicles.Select(vehicle => new ExportVehicle()
                {
                    Id = vehicle.Id.Value,
                    Name = vehicle.Name.Value,
                    Price = vehicle.Price.Value,
                    MileageInKilometers = vehicle.MileageInKilometers.ValueInKilometers,
                    Color = vehicle.Color.Value,
                    RegistrationNumber = vehicle.RegistrationNumber.Value,
                    PurchaseDate = vehicle.PurchaseDate,
                    BrandModelId = vehicle.BrandModelId.Value,
                    BrandName = vehicle.BrandModel?.BrandName,
                    ModelName = vehicle.BrandModel?.ModelName,
                    EnterpriseId = vehicle.EnterpriseId.Value,
                    ActiveDriverId = vehicle.ActiveDriverId?.Value
                }).ToArray(),
                // Vehicles = enterprise.Vehicles.Select(v => new ExportVehicle
                // {
                //     Id = v.Id.Value,
                //     Name = v.Name.Value,
                //     Price = v.Price.Value,
                //     MileageInKilometers = v.MileageInKilometers.Value,
                //     Color = v.Color.Value,
                //     RegistrationNumber = v.RegistrationNumber.Value,
                //     PurchaseDate = v.PurchaseDate,
                //     BrandModelId = v.BrandModelId.Value,
                //     BrandName = v.BrandModel?.BrandName,
                //     ModelName = v.BrandModel?.Name?.Value,
                //     EnterpriseId = v.EnterpriseId.Value,
                //     ActiveDriverId = v.ActiveDriverId?.Value
                // }).ToArray(),
                Drivers = enterprise.Drivers.Select(d => new ExportDriver
                {
                    Id = d.Id.Value,
                    FirstName = d.User?.FirstName?.Value ?? "",
                    LastName = d.User?.LastName?.Value ?? "",
                    Salary = d.Salary,
                    EnterpriseId = d.EnterpriseId.Value
                }).ToArray(),
                Trips = tripsList.Select(t => new ExportTrip
                {
                    Id = t.Id.Value,
                    VehicleId = t.VehicleId.Value,
                    StartUtc = t.StartUtc,
                    EndUtc = t.EndUtc,
                    DistanceKm = t.DistanceKm,
                    StartLatitude = t.StartPoint?.Latitude,
                    StartLongitude = t.StartPoint?.Longitude,
                    StartAddress = t.StartPoint?.Address,
                    EndLatitude = t.EndPoint?.Latitude,
                    EndLongitude = t.EndPoint?.Longitude,
                    EndAddress = t.EndPoint?.Address
                }).ToArray(),
                TrackPoints = trackPointsList.Select(tp => new ExportTrackPoint
                {
                    VehicleId = tp.VehicleId.Value,
                    Timestamp = tp.TimestampUtc,
                    Latitude = tp.Latitude,
                    Longitude = tp.Longitude,
                    Speed = tp.Speed,
                    Rpm = tp.Rpm,
                    FuelLevel = tp.FuelLevel
                }).ToArray(),
                ExportedAt = DateTime.UtcNow,
                DateRange = new ExportDateRange
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }
            };

            string content;
            string contentType;
            string fileName;

            if (request.Format.ToLower() == "csv")
            {
                content = ConvertToCsv(exportData);
                contentType = "text/csv";
                fileName = $"enterprise_{request.EnterpriseId}_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            }
            else
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                content = JsonSerializer.Serialize(exportData, options);
                contentType = "application/json";
                fileName = $"enterprise_{request.EnterpriseId}_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            }

            return new ExportEnterpriseDataResponse(content, contentType, fileName);
        }
        catch (Exception ex)
        {
            return Error.New($"Error exporting enterprise data: {ex.Message}");
        }
    }

    private string ConvertToCsv(ExportData data)
    {
        var csv = new StringBuilder();

        // Enterprise info
        csv.AppendLine("=== ENTERPRISE ===");
        csv.AppendLine("Id,Name,Address,TimeZoneId");
        csv.AppendLine($"{data.Enterprise.Id},\"{data.Enterprise.Name}\",\"{data.Enterprise.Address}\",\"{data.Enterprise.TimeZoneId}\"");
        csv.AppendLine();

        // Vehicles
        csv.AppendLine("=== VEHICLES ===");
        csv.AppendLine("Id,Name,Price,MileageInKilometers,Color,RegistrationNumber,PurchaseDate,BrandModelId,BrandName,ModelName,EnterpriseId,ActiveDriverId");
        foreach (var vehicle in data.Vehicles)
        {
            csv.AppendLine($"{vehicle.Id},\"{vehicle.Name}\",{vehicle.Price},{vehicle.MileageInKilometers},\"{vehicle.Color}\",\"{vehicle.RegistrationNumber}\",{vehicle.PurchaseDate?.ToString("yyyy-MM-dd HH:mm:ss")},{vehicle.BrandModelId},\"{vehicle.BrandName}\",\"{vehicle.ModelName}\",{vehicle.EnterpriseId},{vehicle.ActiveDriverId}");
        }
        csv.AppendLine();

        // Drivers
        csv.AppendLine("=== DRIVERS ===");
        csv.AppendLine("Id,FirstName,LastName,Salary,EnterpriseId");
        foreach (var driver in data.Drivers)
        {
            csv.AppendLine($"{driver.Id},\"{driver.FirstName}\",\"{driver.LastName}\",{driver.Salary},{driver.EnterpriseId}");
        }
        csv.AppendLine();

        // Trips
        csv.AppendLine("=== TRIPS ===");
        csv.AppendLine("Id,VehicleId,StartUtc,EndUtc,DistanceKm,StartLatitude,StartLongitude,StartAddress,EndLatitude,EndLongitude,EndAddress");
        foreach (var trip in data.Trips)
        {
            csv.AppendLine($"{trip.Id},{trip.VehicleId},{trip.StartUtc:yyyy-MM-dd HH:mm:ss},{trip.EndUtc:yyyy-MM-dd HH:mm:ss},{trip.DistanceKm?.ToString(CultureInfo.InvariantCulture)},{trip.StartLatitude?.ToString(CultureInfo.InvariantCulture)},{trip.StartLongitude?.ToString(CultureInfo.InvariantCulture)},\"{trip.StartAddress}\",{trip.EndLatitude?.ToString(CultureInfo.InvariantCulture)},{trip.EndLongitude?.ToString(CultureInfo.InvariantCulture)},\"{trip.EndAddress}\"");
        }
        csv.AppendLine();

        // Track Points
        csv.AppendLine("=== TRACK_POINTS ===");
        csv.AppendLine("VehicleId,Timestamp,Latitude,Longitude,Speed,Rpm,FuelLevel");
        foreach (var point in data.TrackPoints)
        {
            csv.AppendLine($"{point.VehicleId},{point.Timestamp:yyyy-MM-dd HH:mm:ss},{point.Latitude.ToString(CultureInfo.InvariantCulture)},{point.Longitude.ToString(CultureInfo.InvariantCulture)},{point.Speed.ToString(CultureInfo.InvariantCulture)},{point.Rpm},{point.FuelLevel}");
        }

        return csv.ToString();
    }
}

// Data models for export
public class ExportData
{
    public ExportEnterprise Enterprise { get; set; } = null!;
    public ExportVehicle[] Vehicles { get; set; } = Array.Empty<ExportVehicle>();
    public ExportDriver[] Drivers { get; set; } = Array.Empty<ExportDriver>();
    public ExportTrip[] Trips { get; set; } = Array.Empty<ExportTrip>();
    public ExportTrackPoint[] TrackPoints { get; set; } = Array.Empty<ExportTrackPoint>();
    public DateTime ExportedAt { get; set; }
    public ExportDateRange DateRange { get; set; } = null!;
}

public class ExportEnterprise
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? TimeZoneId { get; set; }
}

public class ExportVehicle
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public double MileageInKilometers { get; set; }
    public string Color { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public DateTimeOffset? PurchaseDate { get; set; }
    public int BrandModelId { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public int EnterpriseId { get; set; }
    public int? ActiveDriverId { get; set; }
}

public class ExportDriver
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public decimal Salary { get; set; }
    public int EnterpriseId { get; set; }
}

public class ExportTrip
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public double? DistanceKm { get; set; }
    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
    public string? StartAddress { get; set; }
    public double? EndLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public string? EndAddress { get; set; }
}

public class ExportTrackPoint
{
    public int VehicleId { get; set; }
    public DateTime Timestamp { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float Speed { get; set; }
    public ushort Rpm { get; set; }
    public byte FuelLevel { get; set; }
}

public class ExportDateRange
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}