using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Manager.Entities;
using Autopark.Domain.Manager.ValueObjects;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Autopark.Infrastructure.Database;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autopark.UseCases.Manager.Commands.Create;

internal class CreateManagerCommandHandler : IRequestHandler<CreateManagerCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public CreateManagerCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<LanguageExt.Unit>> Handle(CreateManagerCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(request.UserId);
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return Error.New($"Пользователь с идентификатором '{userId.Value}' не существует");

        // Проверяем, есть ли уже ManagerEntity для этого пользователя
        var manager = await _dbContext.Managers.FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
        if (manager == null)
        {
            manager = new ManagerEntity
            {
                UserId = userId
            };
            await _dbContext.Managers.AddAsync(manager, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken); // чтобы получить manager.Id
        }

        // Назначаем роль Manager, если её ещё нет
        var isAlreadyManager = user.Roles.Any(r => r.Role == UserRoleType.Manager);
        if (!isAlreadyManager)
        {
            var managerRole = new UserRole
            {
                UserId = userId,
                Role = UserRoleType.Manager
            };
            await _dbContext.UserRoles.AddAsync(managerRole, cancellationToken);
        }

        // Для каждого предприятия создаём связь, если её нет
        foreach (var enterpriseIdInt in request.EnterpriseIds.Distinct())
        {
            var enterpriseId = Autopark.Domain.Enterprise.ValueObjects.EnterpriseId.Create(enterpriseIdInt);
            var exists = await _dbContext.ManagerEnterprises.AnyAsync(me => me.ManagerId == manager.Id && me.EnterpriseId == enterpriseId, cancellationToken);
            if (!exists)
            {
                var managerEnterprise = ManagerEnterpriseEntity.Create(manager.Id, enterpriseId);
                await _dbContext.ManagerEnterprises.AddAsync(managerEnterprise, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return LanguageExt.Unit.Default;
    }
}