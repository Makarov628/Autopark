using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.Vehicle.ValueObjects;
using LanguageExt.Common;
using Autopark.Infrastructure.Database.Identity;
using Autopark.Infrastructure.Database.Services;

namespace Autopark.UseCases.Vehicle.Queries.GetById;

internal class GetByIdVehicleQueryHandler : IRequestHandler<GetByIdVehicleQuery, Fin<VehicleResponse>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ITimeZoneService _timeZoneService;

    public GetByIdVehicleQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser, ITimeZoneService timeZoneService)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _timeZoneService = timeZoneService;
    }

    public async Task<Fin<VehicleResponse>> Handle(GetByIdVehicleQuery request, CancellationToken cancellationToken)
    {
        var vehicleId = VehicleId.Create(request.Id);
        var vehicle = await _dbContext.Vehicles
            .Include(v => v.Enterprise)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == vehicleId && _currentUser.EnterpriseIds.Contains(v.EnterpriseId), cancellationToken);

        if (vehicle is null)
            return Error.New($"Vehicle not found with id: {request.Id}");

        var driversIds = await _dbContext.Drivers
            .AsNoTracking()
            .Where(d => d.EnterpriseId == vehicle.EnterpriseId && d.VehicleId == vehicle.Id)
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        // Конвертируем время покупки в таймзону предприятия
        var purchaseDateInEnterpriseTime = vehicle.PurchaseDate.HasValue
            ? _timeZoneService.ToEnterpriseZone(vehicle.PurchaseDate.Value, vehicle.Enterprise.TimeZoneId)
            : (DateTimeOffset?)null;

        return new VehicleResponse(
            vehicle.Id.Value,
            vehicle.Name.Value,
            vehicle.Price.Value,
            vehicle.MileageInKilometers.ValueInKilometers,
            vehicle.Color.Value,
            vehicle.RegistrationNumber.Value,
            vehicle.BrandModelId.Value,
            vehicle.EnterpriseId.Value,
            vehicle.ActiveDriverId?.Value,
            purchaseDateInEnterpriseTime,
            driversIds.Select(d => d.Value).ToArray()
        );
    }
}
