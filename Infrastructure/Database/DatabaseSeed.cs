

using Autopark.Infrastructure.Database.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autopark.Infrastructure.Database;

public static class DatabaseSeed
{
    public static async Task SeedAdminAsync(IServiceScope scope)
    {
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        const string adminRole = "Admin";
        if (!await roles.RoleExistsAsync(adminRole))
            await roles.CreateAsync(new IdentityRole(adminRole));

        var email = cfg["Admin:Email"] ?? "admin@example.com";
        var user = await users.FindByEmailAsync(email);
        if (user is not null)
            return;

        user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            UserName = email,
            IsPasswordInitialized = false
        };

        var randomPassword = Guid.NewGuid() + "aA!";
        await users.CreateAsync(user, randomPassword);
        await users.AddToRoleAsync(user, adminRole);

    }
}