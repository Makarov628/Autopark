using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.Enterprise.ValueObjects;
using LanguageExt.Common;

namespace Autopark.UseCases.Enterprise.Queries.GetById;

internal class GetByIdEnterpriseQueryHandler : IRequestHandler<GetByIdEnterpriseQuery, Fin<EnterpriseResponse>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetByIdEnterpriseQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<EnterpriseResponse>> Handle(GetByIdEnterpriseQuery request, CancellationToken cancellationToken)
    {
        var enterpriseId = EnterpriseId.Create(request.Id);
        var enterprise = await _dbContext.Enterprises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == enterpriseId, cancellationToken);
        if (enterprise is null)
            return Error.New($"Enterprise not found with id: {request.Id}");

        var vehicles = await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.EnterpriseId == enterpriseId)
            .ToListAsync(cancellationToken);

        var drivers = await _dbContext.Drivers
            .AsNoTracking()
            .Where(d => d.EnterpriseId == enterpriseId)
            .ToListAsync(cancellationToken);

        var brandModelIds = vehicles.Select(v => v.BrandModelId).ToList();
        var brandModels = await _dbContext.BrandModels
            .AsNoTracking()
            .Where(bm => brandModelIds.Contains(bm.Id))
            .ToListAsync(cancellationToken);

        return new EnterpriseResponse(
            enterprise.Id.Value,
            enterprise.Name.Value,
            enterprise.Address,
            vehicles.ConvertAll(vehicle => new Vehicle(
                vehicle.Id.Value,
                vehicle.Name.Value,
                vehicle.Price.Value,
                vehicle.MileageInKilometers.ValueInKilometers,
                vehicle.Color.Value,
                vehicle.RegistrationNumber.Value,
                brandModels.Where(bm => bm.Id == vehicle.BrandModelId).Select(bm => new BrandModel(
                    bm.Id.Value,
                    bm.BrandName,
                    bm.ModelName,
                    bm.TransportType,
                    bm.FuelType,
                    bm.SeatsNumber,
                    bm.MaximumLoadCapacityInKillograms
                )).First(),
                vehicle.ActiveDriverId != null
                    ? drivers.Select(d => new Driver(
                        d.Id.Value,
                        d.FirstName.Value,
                        d.LastName.Value,
                        d.DateOfBirth,
                        d.Salary
                    )).FirstOrDefault(d => d.Id == vehicle.ActiveDriverId.Value)
                    : null,
                drivers.Where(d => d.VehicleId is not null && d.VehicleId == vehicle.Id).Select(d => new Driver(
                    d.Id.Value,
                    d.FirstName.Value,
                    d.LastName.Value,
                    d.DateOfBirth,
                    d.Salary
                )).ToArray()
            )).ToArray(),
            drivers.Select(d => new Driver(
                d.Id.Value,
                d.FirstName.Value,
                d.LastName.Value,
                d.DateOfBirth,
                d.Salary,
                d.VehicleId is not null ? vehicles.Where(v => v.Id == d.VehicleId).Select(v => new VehicleForDriver(
                    v.Id.Value,
                    v.Name.Value,
                    v.Price.Value,
                    v.MileageInKilometers.ValueInKilometers,
                    v.Color.Value,
                    v.RegistrationNumber.Value,
                    brandModels.Where(bm => bm.Id == v.BrandModelId).Select(bm => new BrandModel(
                        bm.Id.Value,
                        bm.BrandName,
                        bm.ModelName,
                        bm.TransportType,
                        bm.FuelType,
                        bm.SeatsNumber,
                        bm.MaximumLoadCapacityInKillograms
                    )).First(),
                    v.ActiveDriverId?.Value)
                ).First()
                : null
            )
            ).ToArray());
    }
}