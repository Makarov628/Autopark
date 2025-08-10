
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Autopark.Infrastructure.Database;

public class AutoparkDbContextFactory
    : IDesignTimeDbContextFactory<AutoparkDbContext>
{
    public AutoparkDbContext CreateDbContext(string[] args)
    {
        return Create(Directory.GetCurrentDirectory(),
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development");
    }

    private AutoparkDbContext CreateNewInstance(DbContextOptions<AutoparkDbContext> options)
    {
        return new AutoparkDbContext(options);
    }

    private AutoparkDbContext Create(string basePath, string env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Could not found a connection string named 'DefaultConnection'");

        return Create(connectionString);
    }

    private AutoparkDbContext Create(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException($"{nameof(connectionString)} is null or empty", nameof(connectionString));

        var optionsBuilder = new DbContextOptionsBuilder<AutoparkDbContext>();
        optionsBuilder.UseSqlServer(connectionString, x => x.UseNetTopologySuite());

        var options = optionsBuilder.Options;
        return CreateNewInstance(options);
    }
}
