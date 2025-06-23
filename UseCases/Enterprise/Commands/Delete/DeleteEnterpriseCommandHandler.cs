using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.UseCases.Common.Exceptions;

namespace Autopark.UseCases.Enterprise.Commands.Delete;

internal class DeleteEnterpriseCommandHandler : IRequestHandler<DeleteEnterpriseCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public DeleteEnterpriseCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(DeleteEnterpriseCommand request, CancellationToken cancellationToken)
    {
        var enterpriseId = EnterpriseId.Create(request.Id);
        var enterprise = await _dbContext.Enterprises.AsNoTracking().FirstOrDefaultAsync(v => v.Id == enterpriseId, cancellationToken);
        if (enterprise is null)
            return Error.New($"Enterprise not found with id: {request.Id}");

        // Проверяем наличие связанных сущностей
        var hasVehicles = await _dbContext.Vehicles.AnyAsync(v => v.EnterpriseId == enterpriseId, cancellationToken);
        if (hasVehicles)
            throw new ConflictException($"Предприятие с идентификатором '{enterpriseId.Value}' не может быть удалено, так как используется в транспортных средствах");

        var hasDrivers = await _dbContext.Drivers.AnyAsync(d => d.EnterpriseId == enterpriseId, cancellationToken);
        if (hasDrivers)
            throw new ConflictException($"Предприятие с идентификатором '{enterpriseId.Value}' не может быть удалено, так как связано с водителями");

        var hasManagers = await _dbContext.ManagerEnterprises.AnyAsync(me => me.EnterpriseId == enterpriseId, cancellationToken);
        if (hasManagers)
            throw new ConflictException($"Предприятие с идентификатором '{enterpriseId.Value}' не может быть удалено, так как связано с менеджерами");

        if (_dbContext.Entry(enterprise).State == EntityState.Detached)
            _dbContext.Enterprises.Attach(enterprise);

        _dbContext.Enterprises.Remove(enterprise);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}