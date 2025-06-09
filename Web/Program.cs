using Autopark.Infrastructure.Database;
using Autopark.UseCases.Vehicle.Commands.Create;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Autopark.Domain.Manager.Entities;
using Autopark.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Autopark.Infrastructure.Database.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Autopark", Version = "v1" });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddCors();

builder.Services.AddIdentityCore<ManagerEntity>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AutoparkDbContext>()
    .AddApiEndpoints();

builder.Services.AddDbContext<AutoparkDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        });
});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateVehicleCommand).Assembly));
builder.Services.AddTransient<IClaimsTransformation, EnterpriseClaimsTransformation>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapIdentityApi<ManagerEntity>();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AutoparkDbContext>();
    dbContext.Database.Migrate();
    await DatabaseSeed.SeedAdminAsync(scope);
}

app.Run();
