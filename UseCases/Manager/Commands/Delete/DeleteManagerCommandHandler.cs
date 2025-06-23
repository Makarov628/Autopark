using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Manager.ValueObjects;
using Autopark.Domain.User.Entities;

namespace Autopark.UseCases.Manager.Commands.Delete;

internal class DeleteManagerCommandHandler : IRequestHandler<DeleteManagerCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public DeleteManagerCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(DeleteManagerCommand request, CancellationToken cancellationToken)
    {
        var manager = await _dbContext.Managers
            .Include(m => m.User)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(m => m.Id == ManagerId.Create(request.Id), cancellationToken);

        if (manager is null)
            return Error.New($"Manager not found with id: {request.Id}");

        // Удаляем роль Manager у пользователя
        var managerRole = manager.User.Roles.FirstOrDefault(r => r.Role == UserRoleType.Manager);
        if (managerRole != null)
        {
            _dbContext.UserRoles.Remove(managerRole);
        }

        // Удаляем менеджера
        _dbContext.Managers.Remove(manager);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}