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
        var vehicleId = VehicleId.Create(request.Id);
        var vehicle = await _dbContext.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);
        if (vehicle is null)
            return Error.New($"Vehicle not found with id: {request.Id}");

        // Проверяем наличие связанных водителей
        var hasDrivers = await _dbContext.Drivers.AnyAsync(d => d.VehicleId == vehicleId, cancellationToken);
        if (hasDrivers)
            return Error.New($"Транспортное средство с идентификатором '{vehicleId.Value}' не может быть удалено, так как связано с водителями");

        if (_dbContext.Entry(vehicle).State == EntityState.Detached)
            _dbContext.Vehicles.Attach(vehicle);

        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}