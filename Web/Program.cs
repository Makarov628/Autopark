using Autopark.Infrastructure.Database;
using Autopark.UseCases.Vehicle.Commands.Create;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Autopark.Web.Services;
using Autopark.Web.Middleware;
using Autopark.Infrastructure.Database.Identity;
using Autopark.Infrastructure.Database.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TimeZoneConverter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Autopark", Version = "v1" });

    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Настройка аутентификации
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "your-secret-key-here"))
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = JwtEventsHandler.OnTokenValidated
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors();

// Регистрация инфраструктуры (включая DbContext и все сервисы)
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Autopark.db");

// Удалили регистрацию ClickHouse - теперь используем PostgreSQL через Entity Framework

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateVehicleCommand).Assembly));

// Регистрация сервисов
// builder.Services.AddTransient<IClaimsTransformation, EnterpriseClaimsTransformation>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Добавляем контроллеры и Razor Pages
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Настройка сериализации DateTimeOffset в ISO-8601 формате
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// Используем наш JWT middleware
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Маршрутизация: сначала API контроллеры
app.MapControllers();

// Затем Razor Pages (кабинет менеджера)
app.MapRazorPages();

// // React приложение доступно по пути /app
// app.MapFallbackToFile("/app", "index.html");
// app.MapFallbackToFile("/app/{*path}", "index.html");

// // И только потом React приложение для всех остальных маршрутов (если нужно)
// app.MapFallbackToFile("index.html");

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AutoparkDbContext>();
    if (!builder.Environment.IsEnvironment("Test"))
    {
        dbContext.Database.Migrate();
    }
}

app.Run();

// Частичное объявление для совместимости с тестами
public partial class Program { }
