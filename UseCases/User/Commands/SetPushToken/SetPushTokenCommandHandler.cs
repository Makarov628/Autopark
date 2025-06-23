using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using Autopark.Domain.User.ValueObjects;

namespace Autopark.UseCases.User.Commands.SetPushToken;

public class SetPushTokenCommandHandler : IRequestHandler<SetPushTokenCommand, bool>
{
    private readonly AutoparkDbContext _dbContext;

    public SetPushTokenCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(SetPushTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Находим устройство по DeviceId
        var device = await _dbContext.Devices
            .FirstOrDefaultAsync(d => d.Id == DeviceId.Create(request.DeviceId), cancellationToken);

        if (device == null)
        {
            throw new InvalidOperationException("Устройство не найдено");
        }

        // 2. Проверяем, что устройство не истёкло
        if (device.ExpireDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Сессия устройства истёкла");
        }

        // 3. Обновляем PushToken
        device.PushToken = request.PushToken;
        device.LastActive = DateTime.UtcNow;

        // 4. Сохраняем изменения
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}