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
        var vehicle = await _dbContext.Vehicles.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        if (vehicle is null)
            return Error.New($"Vehicle not found with id: {request.Id}");

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
            registrationNumber.Head());

        _dbContext.Vehicles.Attach(vehicle);
        _dbContext.Entry(vehicle).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}