using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Autopark.Infrastructure.Database.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Autopark.UseCases.User.Commands.Activate;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, bool>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public ActivateUserCommandHandler(
        AutoparkDbContext dbContext,
        IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Находим токен активации
        var activationToken = await _dbContext.ActivationTokens
            .Include(at => at.User)
            .ThenInclude(u => u.Credentials)
            .FirstOrDefaultAsync(at => at.Token == request.Token, cancellationToken);

        if (activationToken == null)
        {
            throw new InvalidOperationException("Недействительный токен активации");
        }

        // 2. Проверяем срок действия токена
        if (activationToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Токен активации истёк");
        }

        // 3. Проверяем, что пользователь ещё не активирован
        if (activationToken.User.IsActive)
        {
            throw new InvalidOperationException("Пользователь уже активирован");
        }

        // 4. Проверяем совпадение паролей
        if (request.Password != request.RepeatPassword)
        {
            throw new InvalidOperationException("Пароли не совпадают");
        }

        // 5. Хэшируем пароль
        var salt = _passwordHasher.GenerateSalt();
        var passwordHash = _passwordHasher.HashPassword(request.Password, salt);

        // 6. Обновляем Credentials
        activationToken.User.Credentials.PasswordHash = passwordHash;
        activationToken.User.Credentials.Salt = salt;

        // 7. Активируем пользователя
        activationToken.User.IsActive = true;
        activationToken.User.EmailConfirmed = DateTime.UtcNow;

        // 8. Удаляем токен активации
        _dbContext.ActivationTokens.Remove(activationToken);

        // 9. Сохраняем изменения
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}