using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Autopark.Infrastructure.Database.Services;
using Autopark.Infrastructure.Database.Identity;

namespace Autopark.Infrastructure.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Регистрируем DbContext
        services.AddDbContext<AutoparkDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Регистрируем сервисы
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IMessengerService, MessengerService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ITimeZoneService, TimeZoneService>();

        return services;
    }
}