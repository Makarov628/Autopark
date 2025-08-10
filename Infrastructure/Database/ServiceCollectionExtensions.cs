using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Autopark.Infrastructure.Database.Services;
using Autopark.Infrastructure.Database.Identity;
using NetTopologySuite;

namespace Autopark.Infrastructure.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Регистрируем DbContext с поддержкой NetTopologySuite
        services.AddDbContext<AutoparkDbContext>(options =>
            options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

        // Регистрируем сервисы
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IMessengerService, MessengerService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ITimeZoneService, TimeZoneService>();
        services.AddScoped<IVehicleTrackingService, VehicleTrackingService>();
        services.AddScoped<ITripService, TripService>();

        // Регистрируем HTTP клиент для геокодирования
        services.AddHttpClient<IGeocodingService, GeoapifyGeocodingService>();

        return services;
    }
}