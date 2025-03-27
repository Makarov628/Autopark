using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Vehicle.ValueObjects;

namespace Autopark.UseCases.Vehicle.Commands.Delete;

internal class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public DeleteVehicleCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _dbContext.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == VehicleId.Create(request.Id), cancellationToken);
        if (vehicle is null)
            return Error.New($"Vehicle not found with id: {request.Id}");

        if (_dbContext.Entry(vehicle).State == EntityState.Detached)
            _dbContext.Vehicles.Attach(vehicle);

        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}