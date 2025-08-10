using System.Globalization;
using System.Text.Json;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Autopark.Domain.Trip.Entities;
using Autopark.Domain.Trip.ValueObjects;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.UseCases.Enterprise.Queries.ExportData;
using System.Text;

namespace Autopark.UseCases.Enterprise.Commands.ImportData;

public class ImportEnterpriseDataCommandHandler : IRequestHandler<ImportEnterpriseDataCommand, Fin<ImportEnterpriseDataResponse>>
{
    private readonly AutoparkDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly GeometryFactory _geometryFactory;

    public ImportEnterpriseDataCommandHandler(AutoparkDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
        _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
    }

    public async Task<Fin<ImportEnterpriseDataResponse>> Handle(ImportEnterpriseDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var warnings = new List<string>();
            var errors = new List<string>();
            int enterprisesImported = 0, vehiclesImported = 0, driversImported = 0, tripsImported = 0, trackPointsImported = 0;

            ExportData? importData;

            if (request.Format.ToLower() == "csv")
            {
                importData = ParseCsvData(request.Content, warnings, errors);
            }
            else
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                importData = JsonSerializer.Deserialize<ExportData>(request.Content, options);
            }

            if (importData == null)
            {
                return Error.New("Failed to parse import data");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Import Enterprise
                if (importData.Enterprise != null)
                {
                    var existingEnterprise = await _context.Enterprises
                        .FirstOrDefaultAsync(e => e.Id == EnterpriseId.Create(importData.Enterprise.Id), cancellationToken);

                    if (existingEnterprise == null)
                    {
                        var nameResult = CyrillicString.Create(importData.Enterprise.Name);
                        if (nameResult.IsFail)
                        {
                            errors.Add($"Invalid enterprise name: {nameResult.Match(_ => "", error => error.Message)}");
                            return new ImportEnterpriseDataResponse(0, 0, 0, 0, 0, warnings.ToArray(), errors.ToArray());
                        }

                        var enterprise = EnterpriseEntity.Create(
                            EnterpriseId.Create(importData.Enterprise.Id),
                            nameResult.Match(success => success, error => throw new Exception(error.Message)),
                            importData.Enterprise.Address,
                            importData.Enterprise.TimeZoneId
                        );
                        _context.Enterprises.Add(enterprise);
                        enterprisesImported++;
                    }
                    else if (request.UpdateExisting)
                    {
                        var nameResult = CyrillicString.Create(importData.Enterprise.Name);
                        if (nameResult.IsFail)
                        {
                            errors.Add($"Invalid enterprise name: {nameResult.Match(_ => "", error => error.Message)}");
                            return new ImportEnterpriseDataResponse(0, 0, 0, 0, 0, warnings.ToArray(), errors.ToArray());
                        }

                        existingEnterprise.Update(
                            nameResult.Match(success => success, error => throw new Exception(error.Message)),
                            importData.Enterprise.Address,
                            importData.Enterprise.TimeZoneId
                        );
                        enterprisesImported++;
                    }
                    else
                    {
                        warnings.Add($"Enterprise with ID {importData.Enterprise.Id} already exists and was skipped");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Import Vehicles
                foreach (var vehicleData in importData.Vehicles)
                {
                    try
                    {
                        var existingVehicle = await _context.Vehicles
                            .FirstOrDefaultAsync(v => v.Id == VehicleId.Create(vehicleData.Id), cancellationToken);

                        if (existingVehicle == null)
                        {
                            var nameResult = CyrillicString.Create(vehicleData.Name);
                            var priceResult = Price.Create(vehicleData.Price);
                            var mileageResult = Mileage.Create(vehicleData.MileageInKilometers);
                            var colorResult = CyrillicString.Create(vehicleData.Color);
                            var regNumberResult = RegistrationNumber.Create(vehicleData.RegistrationNumber);

                            if (nameResult.IsFail || priceResult.IsFail || mileageResult.IsFail ||
                                colorResult.IsFail || regNumberResult.IsFail)
                            {
                                errors.Add($"Invalid vehicle data for ID {vehicleData.Id}");
                                continue;
                            }

                            var vehicle = VehicleEntity.Create(
                                VehicleId.Create(vehicleData.Id),
                                nameResult.Match(success => success, error => throw new Exception(error.Message)),
                                priceResult.Match(success => success, error => throw new Exception(error.Message)),
                                mileageResult.Match(success => success, error => throw new Exception(error.Message)),
                                colorResult.Match(success => success, error => throw new Exception(error.Message)),
                                regNumberResult.Match(success => success, error => throw new Exception(error.Message)),
                                BrandModelId.Create(vehicleData.BrandModelId),
                                EnterpriseId.Create(vehicleData.EnterpriseId),
                                vehicleData.PurchaseDate
                            );
                            _context.Vehicles.Add(vehicle);
                            vehiclesImported++;
                        }
                        else if (request.UpdateExisting)
                        {
                            var nameResult = CyrillicString.Create(vehicleData.Name);
                            var priceResult = Price.Create(vehicleData.Price);
                            var mileageResult = Mileage.Create(vehicleData.MileageInKilometers);
                            var colorResult = CyrillicString.Create(vehicleData.Color);
                            var regNumberResult = RegistrationNumber.Create(vehicleData.RegistrationNumber);

                            if (nameResult.IsFail || priceResult.IsFail || mileageResult.IsFail ||
                                colorResult.IsFail || regNumberResult.IsFail)
                            {
                                errors.Add($"Invalid vehicle data for ID {vehicleData.Id}");
                                continue;
                            }

                            existingVehicle.Update(
                                nameResult.Match(success => success, error => throw new Exception(error.Message)),
                                priceResult.Match(success => success, error => throw new Exception(error.Message)),
                                mileageResult.Match(success => success, error => throw new Exception(error.Message)),
                                colorResult.Match(success => success, error => throw new Exception(error.Message)),
                                regNumberResult.Match(success => success, error => throw new Exception(error.Message)),
                                BrandModelId.Create(vehicleData.BrandModelId),
                                EnterpriseId.Create(vehicleData.EnterpriseId),
                                vehicleData.ActiveDriverId.HasValue ? DriverId.Create(vehicleData.ActiveDriverId.Value) : null,
                                vehicleData.PurchaseDate
                            );
                            vehiclesImported++;
                        }
                        else
                        {
                            warnings.Add($"Vehicle with ID {vehicleData.Id} already exists and was skipped");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error importing vehicle {vehicleData.Id}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Import Drivers
                foreach (var driverData in importData.Drivers)
                {
                    try
                    {
                        var firstNameResult = CyrillicString.Create(driverData.FirstName);
                        var lastNameResult = CyrillicString.Create(driverData.LastName);

                        if (firstNameResult.IsFail || lastNameResult.IsFail)
                        {
                            errors.Add($"Invalid driver name for ID {driverData.Id}");
                            continue;
                        }

                        var existingDriver = await _context.Drivers
                            .Include(d => d.User)
                            .FirstOrDefaultAsync(d => d.Id == DriverId.Create(driverData.Id), cancellationToken);

                        if (existingDriver == null)
                        {
                            // Создаем пользователя для водителя
                            var user = new UserEntity
                            {
                                Email = $"driver{driverData.Id}@autopark.com", // Временный email
                                FirstName = firstNameResult.Match(success => success, error => throw new Exception(error.Message)),
                                LastName = lastNameResult.Match(success => success, error => throw new Exception(error.Message)),
                                IsActive = true
                            };
                            _context.Users.Add(user);
                            await _context.SaveChangesAsync(cancellationToken);

                            var driver = DriverEntity.Create(
                                DriverId.Create(driverData.Id),
                                user.Id,
                                driverData.Salary,
                                EnterpriseId.Create(driverData.EnterpriseId)
                            );
                            _context.Drivers.Add(driver);
                            driversImported++;
                        }
                        else if (request.UpdateExisting)
                        {
                            // Обновляем данные пользователя
                            if (existingDriver.User != null)
                            {
                                existingDriver.User.FirstName = firstNameResult.Match(success => success, error => throw new Exception(error.Message));
                                existingDriver.User.LastName = lastNameResult.Match(success => success, error => throw new Exception(error.Message));
                            }

                            existingDriver.UpdateSalary(driverData.Salary);
                            existingDriver.UpdateEnterprise(EnterpriseId.Create(driverData.EnterpriseId));
                            driversImported++;
                        }
                        else
                        {
                            warnings.Add($"Driver with ID {driverData.Id} already exists and was skipped");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error importing driver {driverData.Id}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Import Trips
                foreach (var tripData in importData.Trips)
                {
                    try
                    {
                        var existingTrip = await _context.Trips
                            .FirstOrDefaultAsync(t => t.Id == TripId.Create(tripData.Id), cancellationToken);

                        if (existingTrip == null)
                        {
                            TripPointEntity? startPoint = null;
                            TripPointEntity? endPoint = null;

                            // Create start point
                            if (tripData.StartLatitude.HasValue && tripData.StartLongitude.HasValue)
                            {
                                var startAddress = tripData.StartAddress;
                                if (string.IsNullOrEmpty(startAddress) && !string.IsNullOrEmpty(request.GeoapifyApiKey))
                                {
                                    startAddress = await GetAddressFromCoordinatesAsync(
                                        tripData.StartLatitude.Value,
                                        tripData.StartLongitude.Value,
                                        request.GeoapifyApiKey);
                                }

                                startPoint = TripPointEntity.Create(
                                    tripData.StartLatitude.Value,
                                    tripData.StartLongitude.Value,
                                    startAddress,
                                    _geometryFactory
                                );
                                _context.TripPoints.Add(startPoint);
                            }
                            else if (!string.IsNullOrEmpty(tripData.StartAddress) && !string.IsNullOrEmpty(request.GeoapifyApiKey))
                            {
                                var coordinates = await GetCoordinatesFromAddressAsync(tripData.StartAddress, request.GeoapifyApiKey);
                                if (coordinates.HasValue)
                                {
                                    startPoint = TripPointEntity.Create(
                                        coordinates.Value.lat,
                                        coordinates.Value.lon,
                                        tripData.StartAddress,
                                        _geometryFactory
                                    );
                                    _context.TripPoints.Add(startPoint);
                                }
                            }

                            // Create end point
                            if (tripData.EndLatitude.HasValue && tripData.EndLongitude.HasValue)
                            {
                                var endAddress = tripData.EndAddress;
                                if (string.IsNullOrEmpty(endAddress) && !string.IsNullOrEmpty(request.GeoapifyApiKey))
                                {
                                    endAddress = await GetAddressFromCoordinatesAsync(
                                        tripData.EndLatitude.Value,
                                        tripData.EndLongitude.Value,
                                        request.GeoapifyApiKey);
                                }

                                endPoint = TripPointEntity.Create(
                                    tripData.EndLatitude.Value,
                                    tripData.EndLongitude.Value,
                                    endAddress,
                                    _geometryFactory
                                );
                                _context.TripPoints.Add(endPoint);
                            }
                            else if (!string.IsNullOrEmpty(tripData.EndAddress) && !string.IsNullOrEmpty(request.GeoapifyApiKey))
                            {
                                var coordinates = await GetCoordinatesFromAddressAsync(tripData.EndAddress, request.GeoapifyApiKey);
                                if (coordinates.HasValue)
                                {
                                    endPoint = TripPointEntity.Create(
                                        coordinates.Value.lat,
                                        coordinates.Value.lon,
                                        tripData.EndAddress,
                                        _geometryFactory
                                    );
                                    _context.TripPoints.Add(endPoint);
                                }
                            }

                            await _context.SaveChangesAsync(cancellationToken);

                            var trip = TripEntity.Create(
                                TripId.Create(tripData.Id),
                                VehicleId.Create(tripData.VehicleId),
                                tripData.StartUtc,
                                tripData.EndUtc,
                                tripData.DistanceKm,
                                startPoint?.Id,
                                endPoint?.Id
                            );
                            _context.Trips.Add(trip);
                            tripsImported++;
                        }
                        else
                        {
                            warnings.Add($"Trip with ID {tripData.Id} already exists and was skipped");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error importing trip {tripData.Id}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Import Track Points
                foreach (var trackPointData in importData.TrackPoints)
                {
                    try
                    {
                        var existingTrackPoint = await _context.VehicleTrackPoints
                            .FirstOrDefaultAsync(tp => tp.VehicleId == VehicleId.Create(trackPointData.VehicleId) &&
                                                     tp.TimestampUtc == trackPointData.Timestamp, cancellationToken);

                        if (existingTrackPoint == null)
                        {
                            var trackPoint = VehicleTrackPointEntity.Create(
                                VehicleId.Create(trackPointData.VehicleId),
                                trackPointData.Timestamp,
                                trackPointData.Latitude,
                                trackPointData.Longitude,
                                trackPointData.Speed,
                                trackPointData.Rpm,
                                trackPointData.FuelLevel
                            );
                            _context.VehicleTrackPoints.Add(trackPoint);
                            trackPointsImported++;
                        }
                        else if (request.UpdateExisting)
                        {
                            // Добавим метод Update в VehicleTrackPointEntity если его нет
                            warnings.Add($"Track point for vehicle {trackPointData.VehicleId} at {trackPointData.Timestamp} already exists and was skipped (update not supported)");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error importing track point for vehicle {trackPointData.VehicleId} at {trackPointData.Timestamp}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new ImportEnterpriseDataResponse(
                    enterprisesImported,
                    vehiclesImported,
                    driversImported,
                    tripsImported,
                    trackPointsImported,
                    warnings.ToArray(),
                    errors.ToArray()
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Error.New($"Transaction failed: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return Error.New($"Error importing enterprise data: {ex.Message}");
        }
    }

    private ExportData? ParseCsvData(string csvContent, List<string> warnings, List<string> errors)
    {
        try
        {
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var exportData = new ExportData
            {
                Enterprise = new ExportEnterprise(),
                Vehicles = Array.Empty<ExportVehicle>(),
                Drivers = Array.Empty<ExportDriver>(),
                Trips = Array.Empty<ExportTrip>(),
                TrackPoints = Array.Empty<ExportTrackPoint>(),
                ExportedAt = DateTime.UtcNow,
                DateRange = new ExportDateRange()
            };

            var currentSection = "";
            var vehicles = new List<ExportVehicle>();
            var drivers = new List<ExportDriver>();
            var trips = new List<ExportTrip>();
            var trackPoints = new List<ExportTrackPoint>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (line.StartsWith("=== ") && line.EndsWith(" ==="))
                {
                    currentSection = line.Replace("=== ", "").Replace(" ===", "");
                    continue;
                }

                if (string.IsNullOrEmpty(line) || line.Contains("Id,Name") || line.Contains("Id,FirstName") || line.Contains("VehicleId,Timestamp"))
                    continue;

                try
                {
                    switch (currentSection)
                    {
                        case "ENTERPRISE":
                            var enterpriseParts = ParseCsvLine(line);
                            if (enterpriseParts.Length >= 3)
                            {
                                exportData.Enterprise = new ExportEnterprise
                                {
                                    Id = int.Parse(enterpriseParts[0]),
                                    Name = enterpriseParts[1],
                                    Address = enterpriseParts[2],
                                    TimeZoneId = enterpriseParts.Length > 3 ? enterpriseParts[3] : null
                                };
                            }
                            break;

                        case "VEHICLES":
                            var vehicleParts = ParseCsvLine(line);
                            if (vehicleParts.Length >= 11)
                            {
                                vehicles.Add(new ExportVehicle
                                {
                                    Id = int.Parse(vehicleParts[0]),
                                    Name = vehicleParts[1],
                                    Price = decimal.Parse(vehicleParts[2], CultureInfo.InvariantCulture),
                                    MileageInKilometers = int.Parse(vehicleParts[3]),
                                    Color = vehicleParts[4],
                                    RegistrationNumber = vehicleParts[5],
                                    PurchaseDate = string.IsNullOrEmpty(vehicleParts[6]) ? null : DateTimeOffset.Parse(vehicleParts[6]),
                                    BrandModelId = int.Parse(vehicleParts[7]),
                                    BrandName = vehicleParts.Length > 8 ? vehicleParts[8] : null,
                                    ModelName = vehicleParts.Length > 9 ? vehicleParts[9] : null,
                                    EnterpriseId = int.Parse(vehicleParts[10]),
                                    ActiveDriverId = vehicleParts.Length > 11 && !string.IsNullOrEmpty(vehicleParts[11]) ? int.Parse(vehicleParts[11]) : null
                                });
                            }
                            break;

                        case "DRIVERS":
                            var driverParts = ParseCsvLine(line);
                            if (driverParts.Length >= 5)
                            {
                                drivers.Add(new ExportDriver
                                {
                                    Id = int.Parse(driverParts[0]),
                                    FirstName = driverParts[1],
                                    LastName = driverParts[2],
                                    Salary = decimal.Parse(driverParts[3], CultureInfo.InvariantCulture),
                                    EnterpriseId = int.Parse(driverParts[4])
                                });
                            }
                            break;

                        case "TRIPS":
                            var tripParts = ParseCsvLine(line);
                            if (tripParts.Length >= 5)
                            {
                                trips.Add(new ExportTrip
                                {
                                    Id = int.Parse(tripParts[0]),
                                    VehicleId = int.Parse(tripParts[1]),
                                    StartUtc = DateTime.Parse(tripParts[2]),
                                    EndUtc = DateTime.Parse(tripParts[3]),
                                    DistanceKm = string.IsNullOrEmpty(tripParts[4]) ? null : double.Parse(tripParts[4], CultureInfo.InvariantCulture),
                                    StartLatitude = tripParts.Length > 5 && !string.IsNullOrEmpty(tripParts[5]) ? double.Parse(tripParts[5], CultureInfo.InvariantCulture) : null,
                                    StartLongitude = tripParts.Length > 6 && !string.IsNullOrEmpty(tripParts[6]) ? double.Parse(tripParts[6], CultureInfo.InvariantCulture) : null,
                                    StartAddress = tripParts.Length > 7 ? tripParts[7] : null,
                                    EndLatitude = tripParts.Length > 8 && !string.IsNullOrEmpty(tripParts[8]) ? double.Parse(tripParts[8], CultureInfo.InvariantCulture) : null,
                                    EndLongitude = tripParts.Length > 9 && !string.IsNullOrEmpty(tripParts[9]) ? double.Parse(tripParts[9], CultureInfo.InvariantCulture) : null,
                                    EndAddress = tripParts.Length > 10 ? tripParts[10] : null
                                });
                            }
                            break;

                        case "TRACK_POINTS":
                            var trackParts = ParseCsvLine(line);
                            if (trackParts.Length >= 7)
                            {
                                trackPoints.Add(new ExportTrackPoint
                                {
                                    VehicleId = int.Parse(trackParts[0]),
                                    Timestamp = DateTime.Parse(trackParts[1]),
                                    Latitude = double.Parse(trackParts[2], CultureInfo.InvariantCulture),
                                    Longitude = double.Parse(trackParts[3], CultureInfo.InvariantCulture),
                                    Speed = float.Parse(trackParts[4], CultureInfo.InvariantCulture),
                                    Rpm = ushort.Parse(trackParts[5]),
                                    FuelLevel = byte.Parse(trackParts[6])
                                });
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    warnings.Add($"Error parsing line {i + 1} in section {currentSection}: {ex.Message}");
                }
            }

            exportData.Vehicles = vehicles.ToArray();
            exportData.Drivers = drivers.ToArray();
            exportData.Trips = trips.ToArray();
            exportData.TrackPoints = trackPoints.ToArray();

            return exportData;
        }
        catch (Exception ex)
        {
            errors.Add($"Error parsing CSV: {ex.Message}");
            return null;
        }
    }

    private string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }

    private async Task<string?> GetAddressFromCoordinatesAsync(double lat, double lon, string apiKey)
    {
        try
        {
            string url = $"https://api.geoapify.com/v1/geocode/reverse?lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&apiKey={apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("features", out var features) && features.GetArrayLength() > 0)
            {
                var firstFeature = features[0];
                if (firstFeature.TryGetProperty("properties", out var properties))
                {
                    if (properties.TryGetProperty("formatted", out var formatted))
                    {
                        return formatted.GetString();
                    }
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<(double lat, double lon)?> GetCoordinatesFromAddressAsync(string address, string apiKey)
    {
        try
        {
            string url = $"https://api.geoapify.com/v1/geocode/search?text={Uri.EscapeDataString(address)}&apiKey={apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("features", out var features) && features.GetArrayLength() > 0)
            {
                var firstFeature = features[0];
                if (firstFeature.TryGetProperty("geometry", out var geometry))
                {
                    if (geometry.TryGetProperty("coordinates", out var coordinates) && coordinates.GetArrayLength() >= 2)
                    {
                        var lon = coordinates[0].GetDouble();
                        var lat = coordinates[1].GetDouble();
                        return (lat, lon);
                    }
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}