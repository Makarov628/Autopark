using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Autopark.Infrastructure.Database;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autopark.UseCases.Driver.Commands.Create;

internal class CreateDriverCommandHandler : IRequestHandler<CreateDriverCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public CreateDriverCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<LanguageExt.Unit>> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
    {
        // 1. Проверяем существование предприятия
        var enterpriseId = EnterpriseId.Create(request.EnterpriseId);
        var enterpriseExists = await _dbContext.Enterprises.AnyAsync(e => e.Id == enterpriseId, cancellationToken);
        if (!enterpriseExists)
            return Error.New($"Предприятие с идентификатором '{enterpriseId.Value}' не существует");

        // 2. Проверяем существование пользователя
        var userId = UserId.Create(request.UserId);
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return Error.New($"Пользователь с идентификатором '{userId.Value}' не существует");

        // 3. Проверяем, что пользователь не является водителем
        var isAlreadyDriver = user.Roles.Any(r => r.Role == UserRoleType.Driver);
        if (isAlreadyDriver)
            return Error.New($"Пользователь уже является водителем");

        // 4. Создаём сущность водителя
        var driver = DriverEntity.Create(
            id: DriverId.Empty, // EF Core сгенерирует Id
            userId: userId,
            salary: request.Salary,
            enterpriseId: enterpriseId
        );

        // 5. Назначаем роль Driver
        var driverRole = new UserRole
        {
            UserId = userId,
            Role = UserRoleType.Driver
        };

        await _dbContext.Drivers.AddAsync(driver, cancellationToken);
        await _dbContext.UserRoles.AddAsync(driverRole, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return LanguageExt.Unit.Default;
    }
}