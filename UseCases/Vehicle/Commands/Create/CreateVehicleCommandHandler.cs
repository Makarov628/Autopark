using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Vehicle.Entities;
using Unit = LanguageExt.Unit;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using LanguageExt.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;

namespace Autopark.UseCases.Vehicle.Commands.Create;

internal class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public CreateVehicleCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var brandModelId = BrandModelId.Create(request.BrandModelId);
        var brandModelExists = await _dbContext.BrandModels.AnyAsync(b => b.Id == brandModelId, cancellationToken);
        if (!brandModelExists)
            return Error.New($"Модель бренда машины с идентификатором '{brandModelId.Value}' не существует");

        var enterpriseId = EnterpriseId.Create(request.EnterpriseId);
        var enterpriseExists = await _dbContext.Enterprises.AnyAsync(e => e.Id == enterpriseId, cancellationToken);
        if (!enterpriseExists)
            return Error.New($"Предприятие с идентификатором '{enterpriseId.Value}' не существует");

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

        var vehicle = VehicleEntity.Create(
            name.Head(),
            price.Head(),
            mileage.Head(),
            color.Head(),
            registrationNumber.Head(),
            brandModelId,
            enterpriseId
        );

        await _dbContext.Vehicles.AddAsync(vehicle, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}