using MediatR;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Autopark.Domain.Manager.Entities;
using Autopark.Domain.Manager.ValueObjects;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Infrastructure.Database.Services;
using System;

namespace Autopark.UseCases.System.Commands.InitialSetup;

public class InitialSetupCommandHandler : IRequestHandler<InitialSetupCommand, InitialSetupResponse>
{
    private readonly AutoparkDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public InitialSetupCommandHandler(AutoparkDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<InitialSetupResponse> Handle(InitialSetupCommand request, CancellationToken cancellationToken)
    {
        // Проверяем, есть ли уже администратор
        var hasAdmin = await _context.UserRoles.AnyAsync(r => r.Role == UserRoleType.Admin, cancellationToken);
        if (hasAdmin)
        {
            return new InitialSetupResponse(false, "Система уже настроена. Администратор уже существует.");
        }

        // Проверяем, что email не занят
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUser != null)
        {
            return new InitialSetupResponse(false, "Пользователь с таким email уже существует.");
        }

        // Получаем следующий ID для пользователя
        var nextUserId = await _context.Users.CountAsync(cancellationToken) + 1;
        var userId = UserId.Create(nextUserId);
        var credentialsId = CredentialsId.Create(nextUserId);
        var salt = _passwordHasher.GenerateSalt();
        var hashedPassword = _passwordHasher.HashPassword(request.Password, salt);

        var user = new UserEntity
        {
            Email = request.Email,
            FirstName = CyrillicString.Create(request.FirstName).ThrowIfFail(),
            LastName = CyrillicString.Create(request.LastName).ThrowIfFail(),
            Phone = request.Phone,
            DateOfBirth = DateTime.UtcNow.AddYears(-18), // Минимальный возраст
            IsActive = true,
            EmailConfirmed = DateTime.UtcNow,
            Credentials = new Credentials
            {
                UserId = userId,
                PasswordHash = hashedPassword,
                Salt = salt,
                Type = CredentialsType.Local
            }
        };

        // Добавляем роль Admin
        var nextUserRoleId = await _context.UserRoles.CountAsync(cancellationToken) + 1;
        var userRole = new UserRole
        {
            UserId = userId,
            Role = UserRoleType.Admin
        };
        user.Roles.Add(userRole);

        // Создаем менеджера для администратора
        var nextManagerId = await _context.Managers.CountAsync(cancellationToken) + 1;
        var manager = new ManagerEntity
        {
            UserId = userId,
            User = user
        };

        // Сохраняем в базу данных
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _context.Managers.AddAsync(manager, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new InitialSetupResponse(
            true,
            "Система успешно настроена. Администратор создан.",
            userId.Value
        );
    }
}