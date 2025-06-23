using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Autopark.Infrastructure.Database.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Autopark.UseCases.User.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly ITokenGenerator _tokenGenerator;

    public RefreshTokenCommandHandler(
        AutoparkDbContext dbContext,
        IJwtService jwtService,
        ITokenGenerator tokenGenerator)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Находим устройство по refresh token
        // В реальной реализации здесь нужно будет хэшировать refresh token для поиска
        // Пока используем простой поиск по токену
        var device = await _dbContext.Devices
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.RefreshTokenHash == request.RefreshToken, cancellationToken);

        if (device == null)
        {
            throw new InvalidOperationException("Недействительный refresh token");
        }

        // 2. Проверяем срок действия refresh token
        if (device.ExpireDate < DateTime.UtcNow)
        {
            // Удаляем просроченное устройство
            _dbContext.Devices.Remove(device);
            await _dbContext.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Refresh token истёк");
        }

        // 3. Проверяем, что пользователь активен
        if (!device.User.IsActive)
        {
            throw new InvalidOperationException("Пользователь неактивен");
        }

        // 4. Генерируем новый refresh token
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenGenerator.GenerateToken(32);

        // 5. Обновляем устройство
        device.RefreshTokenHash = newRefreshTokenHash;
        device.LastActive = DateTime.UtcNow;
        device.ExpireDate = DateTime.UtcNow.AddDays(30); // Продлеваем на 30 дней

        // 6. Генерируем новый access token
        var accessToken = _jwtService.GenerateAccessToken(device.User.Id.Value, device.Id.Value);

        // 7. Сохраняем изменения
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 8. Возвращаем ответ
        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken, // Возвращаем новый refresh token клиенту
            DeviceId = device.Id.Value
        };
    }
}