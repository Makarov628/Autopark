using System.Net;
using System.Text;
using System.Text.Json;
using Autopark.UseCases.User.Commands.Login;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Autopark.Infrastructure.Database.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Autopark.Tests.Infrastructure;

public abstract class AuthenticatedTestBase : TestBase
{
    private const string AdminEmail = "admin@test.com";
    private const string ManagerEmail = "manager@test.com";
    private const string UserEmail = "user@test.com";
    private const string DefaultPassword = "Test123!";

    protected async Task<string> GetAdminTokenAsync()
    {
        return await GetOrCreateTokenAsync(AdminEmail, DefaultPassword, UserRoleType.Admin);
    }

    protected async Task<string> GetManagerTokenAsync()
    {
        return await GetOrCreateTokenAsync(ManagerEmail, DefaultPassword, UserRoleType.Manager);
    }

    protected async Task<string> GetUserTokenAsync()
    {
        return await GetOrCreateTokenAsync(UserEmail, DefaultPassword, UserRoleType.Driver);
    }

    protected async Task<string> GetInvalidTokenAsync()
    {
        return "invalid-token";
    }

    private async Task<string> GetOrCreateTokenAsync(string email, string password, UserRoleType role)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AutoparkDbContext>();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<ITokenGenerator>();

        // Найти пользователя
        var user = await db.Users.Include(u => u.Credentials).Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            var firstName = Autopark.Domain.Common.ValueObjects.CyrillicString.Create("Тест").ThrowIfFail();
            var lastName = Autopark.Domain.Common.ValueObjects.CyrillicString.Create("Пользователь").ThrowIfFail();
            var salt = passwordHasher.GenerateSalt();
            user = new UserEntity
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                Credentials = new Credentials
                {
                    Type = CredentialsType.Local,
                    PasswordHash = passwordHasher.HashPassword(password, salt),
                    Salt = salt,
                },
                Roles = new List<UserRole>(),
                Devices = new List<Device>()
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        // Добавить роль если нет
        if (!user.Roles.Any(r => r.Role == role))
        {
            user.Roles.Add(new UserRole { Role = role });
            await db.SaveChangesAsync();
        }
        // Создать Device
        var device = new Device
        {
            UserId = user.Id,
            DeviceName = "Test Device",
            DeviceType = 0,
            RefreshTokenHash = tokenGenerator.GenerateToken(32),
            PushToken = null,
            LastActive = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddDays(30)
        };
        db.Devices.Add(device);
        await db.SaveChangesAsync();
        // Сгенерировать accessToken
        var accessToken = jwtService.GenerateAccessToken(user.Id.Value, device.Id.Value);
        return accessToken;
    }

    protected HttpClient CreateAuthenticatedClient(string token)
    {
        var client = Factory.CreateClient();
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        return client;
    }

    protected async Task<LoginUserResponse?> LoginUserAsync(string email, string password)
    {
        var client = Factory.CreateClient();

        var loginCommand = new LoginUserCommand
        {
            Email = email,
            Password = password,
            DeviceName = "Test Device",
            DeviceType = 0 // Web
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginCommand),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/api/user/login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LoginUserResponse>(responseContent);
        }

        return null;
    }
}