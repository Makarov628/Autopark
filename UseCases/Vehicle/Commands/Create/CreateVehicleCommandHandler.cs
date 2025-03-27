using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Vehicle.Entities;
using Unit = LanguageExt.Unit;

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
        var vehicleResult = VehicleEntity.Create(
            request.Name,
            request.Price,
            request.MileageInKilometers,
            request.Color
        );

        if (vehicleResult.IsFail)
            return vehicleResult.Bind<Unit>(_ => Unit.Default);

        await _dbContext.Vehicles.AddAsync(vehicleResult.Head(), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}