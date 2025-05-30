using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Common;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Driver.ValueObjects;

namespace Autopark.UseCases.Vehicle.Commands.Update;

internal class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public UpdateVehicleCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicleId = VehicleId.Create(request.Id);
        var vehicle = await _dbContext.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);
        if (vehicle is null)
            return Error.New($"Vehicle not found with id: {request.Id}");

        var brandModelId = BrandModelId.Create(request.BrandModelId);
        var brandModelExists = await _dbContext.BrandModels.AnyAsync(b => b.Id == brandModelId, cancellationToken);
        if (!brandModelExists)
            return Error.New($"Модель бренда машины с идентификатором '{brandModelId.Value}' не существует");

        var enterpriseId = EnterpriseId.Create(request.EnterpriseId);
        var enterpriseExists = await _dbContext.Enterprises.AnyAsync(e => e.Id == enterpriseId, cancellationToken);
        if (!enterpriseExists)
            return Error.New($"Предприятие с идентификатором '{enterpriseId.Value}' не существует");

        var driverId = request.ActiveDriverId.HasValue
            ? DriverId.Create(request.ActiveDriverId.Value)
            : null;
        if (driverId is not null && !await _dbContext.Drivers.AnyAsync(d => d.Id == driverId && d.VehicleId == vehicle.Id && d.EnterpriseId == vehicle.EnterpriseId, cancellationToken))
            return Error.New($"Водитель с идентификатором '{driverId.Value}' не существует или не может быть назначен на этот автомобиль");

        var name = CyrillicString.Create(request.Name);
        var price = Price.Create(request.Price);
        var mileage = Mileage.Create(request.MileageInKilometers);
        var color = CyrillicString.Create(request.Color);
        var registrationNumber = RegistrationNumber.Create(request.RegistrationNumber);

        var potentialErrors = new Either<Error, ValueObject>[]
        {
            name.ToValueObjectEither(),
            price.ToValueObjectEither(),
            mileage.ToValueObjectEither(),
            color.ToValueObjectEither(),
            registrationNumber.ToValueObjectEither()
        };

        var aggregatedErrorMessage = potentialErrors
            .MapLeftT(error => error.Message)
            .Lefts()
            .JoinStrings("; ");

        if (!aggregatedErrorMessage.IsNullOrEmpty())
            return Error.New(aggregatedErrorMessage);

        vehicle.Update(
            name.Head(),
            price.Head(),
            mileage.Head(),
            color.Head(),
            registrationNumber.Head(),
            brandModelId,
            enterpriseId,
            driverId);

        _dbContext.Vehicles.Attach(vehicle);
        _dbContext.Entry(vehicle).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}