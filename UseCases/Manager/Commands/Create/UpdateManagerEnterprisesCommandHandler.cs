using Autopark.Domain.Manager.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.User.ValueObjects;
using Autopark.Infrastructure.Database;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Autopark.UseCases.Manager.Commands.Create;

public class UpdateManagerEnterprisesCommandHandler : IRequestHandler<UpdateManagerEnterprisesCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public UpdateManagerEnterprisesCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<LanguageExt.Unit>> Handle(UpdateManagerEnterprisesCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(request.UserId);
        var manager = await _dbContext.Managers
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

        if (manager == null)
            return Error.New($"Менеджер для пользователя {userId.Value} не найден");

        var managerId = manager.Id;
        var current = await _dbContext.ManagerEnterprises
            .Where(me => me.ManagerId == managerId)
            .ToListAsync(cancellationToken);

        var newEnterpriseIds = request.EnterpriseIds.Distinct().ToHashSet();

        // Удалить лишние связи
        var toRemove = current.Where(me => !newEnterpriseIds.Contains(me.EnterpriseId.Value)).ToList();
        _dbContext.ManagerEnterprises.RemoveRange(toRemove);

        // Добавить новые связи
        var existingIds = current.Select(me => me.EnterpriseId.Value).ToHashSet();
        foreach (var eid in newEnterpriseIds.Except(existingIds))
        {
            var enterpriseId = EnterpriseId.Create(eid);
            var managerEnterprise = ManagerEnterpriseEntity.Create(managerId, enterpriseId);
            await _dbContext.ManagerEnterprises.AddAsync(managerEnterprise, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return LanguageExt.Unit.Default;
    }
}