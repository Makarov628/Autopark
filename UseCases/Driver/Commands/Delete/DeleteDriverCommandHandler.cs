using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.User.Entities;

namespace Autopark.UseCases.Driver.Commands.Delete;

internal class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public DeleteDriverCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _dbContext.Drivers
            .Include(d => d.User)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(v => v.Id == DriverId.Create(request.Id), cancellationToken);

        if (driver is null)
            return Error.New($"Driver not found with id: {request.Id}");

        // Удаляем роль Driver у пользователя
        var driverRole = driver.User.Roles.FirstOrDefault(r => r.Role == UserRoleType.Driver);
        if (driverRole != null)
        {
            _dbContext.UserRoles.Remove(driverRole);
        }

        // Удаляем водителя
        _dbContext.Drivers.Remove(driver);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}