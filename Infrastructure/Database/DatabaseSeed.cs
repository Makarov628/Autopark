using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Autopark.Domain.Manager.Entities;
using Autopark.Domain.Manager.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Autopark.Infrastructure.Database;

public static class DatabaseSeed
{
    public static async Task SeedAdminAsync(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AutoparkDbContext>();
        var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var adminEmail = cfg["Admin:Email"] ?? "admin@autopark.com";
        var adminFirstName = cfg["Admin:FirstName"] ?? "Админ";
        var adminLastName = cfg["Admin:LastName"] ?? "Админов";

        // Проверяем, существует ли уже админ
        var existingAdmin = await dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingAdmin != null)
        {
            // Проверяем, есть ли у пользователя роль Admin
            if (existingAdmin.Roles.Any(r => r.Role == UserRoleType.Admin))
            {
                return; // Админ уже существует
            }
        }

        // Создаем пользователя-админа
        var adminUser = new UserEntity
        {
            Email = adminEmail,
            FirstName = CyrillicString.Create(adminFirstName).ThrowIfFail(),
            LastName = CyrillicString.Create(adminLastName).ThrowIfFail(),
            IsActive = true,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        dbContext.Users.Add(adminUser);
        await dbContext.SaveChangesAsync();

        // Добавляем роль Admin
        var adminRole = new UserRole
        {
            UserId = adminUser.Id,
            Role = UserRoleType.Admin
        };

        dbContext.UserRoles.Add(adminRole);

        // Создаем менеджера для админа
        var adminManager = new ManagerEntity
        {
            UserId = adminUser.Id
        };

        dbContext.Managers.Add(adminManager);
        await dbContext.SaveChangesAsync();
    }
}