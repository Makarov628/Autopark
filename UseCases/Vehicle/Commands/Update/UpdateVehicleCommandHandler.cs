using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;

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

        var vehicleUpdateResult = vehicle.Update(
            request.Name,
            request.Price,
            request.MileageInKilometers,
            request.Color);

        if (vehicleUpdateResult.IsFail)
            return vehicleUpdateResult;

        _dbContext.Vehicles.Attach(vehicle);
        _dbContext.Entry(vehicle).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}