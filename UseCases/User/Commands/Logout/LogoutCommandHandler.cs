using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using Autopark.Domain.User.ValueObjects;

namespace Autopark.UseCases.User.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly AutoparkDbContext _dbContext;

    public LogoutCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // 1. Находим устройство по DeviceId
        var device = await _dbContext.Devices
            .FirstOrDefaultAsync(d => d.Id == DeviceId.Create(request.DeviceId), cancellationToken);

        if (device == null)
        {
            // Устройство уже удалено или не существует
            return true;
        }

        // 2. Удаляем устройство (сессию)
        _dbContext.Devices.Remove(device);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}