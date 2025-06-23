using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Autopark.Infrastructure.Database.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Autopark.UseCases.Common.Exceptions;

namespace Autopark.UseCases.User.Commands.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginUserCommandHandler(
        AutoparkDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ITokenGenerator tokenGenerator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Находим пользователя по email
        var user = await _dbContext.Users
            .Include(u => u.Credentials)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedException("Неверный email или пароль");
        }

        // 2. Проверяем, что пользователь активирован
        if (!user.IsActive)
        {
            throw new UnauthorizedException("Пользователь не активирован");
        }

        // 3. Проверяем пароль
        if (user.Credentials?.PasswordHash == null || user.Credentials?.Salt == null)
        {
            throw new UnauthorizedException("Неверный email или пароль");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.Credentials.Salt, user.Credentials.PasswordHash))
        {
            throw new UnauthorizedException("Неверный email или пароль");
        }

        // 4. Создаём устройство (сессию)
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = _tokenGenerator.GenerateToken(32); // Хэш для хранения в БД

        var device = new Device
        {
            UserId = user.Id,
            DeviceName = request.DeviceName,
            DeviceType = request.DeviceType,
            RefreshTokenHash = refreshTokenHash,
            PushToken = null, // Будет установлен позже через отдельную команду
            LastActive = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddDays(30) // Refresh token действителен 30 дней
        };

        _dbContext.Devices.Add(device);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 5. Генерируем access token
        var accessToken = _jwtService.GenerateAccessToken(user.Id.Value, device.Id.Value);

        // 6. Возвращаем ответ
        return new LoginUserResponse
        {
            UserId = user.Id.Value,
            AccessToken = accessToken,
            RefreshToken = refreshToken, // Возвращаем оригинальный refresh token клиенту
            DeviceId = device.Id.Value,
            Email = user.Email,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value
        };
    }
}