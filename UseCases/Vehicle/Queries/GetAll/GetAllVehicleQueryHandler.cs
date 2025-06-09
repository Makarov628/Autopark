using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database.Identity;
namespace Autopark.UseCases.Vehicle.Queries.GetAll;

internal class GetAllVehicleQueryHandler : IRequestHandler<GetAllVehiclesQuery, Fin<List<VehiclesResponse>>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public GetAllVehicleQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Fin<List<VehiclesResponse>>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        return await (
            from vehicle in _dbContext.Vehicles
            where _currentUser.EnterpriseIds.Contains(vehicle.EnterpriseId)
            select new VehiclesResponse(
                vehicle.Id.Value,
                vehicle.Name.Value,
                vehicle.Price.Value,
                vehicle.MileageInKilometers.ValueInKilometers,
                vehicle.Color.Value,
                vehicle.RegistrationNumber.Value,
                vehicle.BrandModelId.Value,
                vehicle.EnterpriseId.Value,
                vehicle.ActiveDriverId != null ? vehicle.ActiveDriverId.Value : null,
                _dbContext.Drivers.AsNoTracking()
                    .Where(d => d.EnterpriseId == vehicle.EnterpriseId && d.VehicleId == vehicle.Id)
                    .Select(d => d.Id.Value)
                    .ToArray()
            )
        ).ToListAsync(cancellationToken);
    }
}
