using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Autopark.Infrastructure.Database;

namespace Autopark.Tests;

/// <summary>
/// Базовый класс для интеграционных тестов.
/// Создаёт единый in-memory SQLite, чтобы все DbContext-ы и HTTP-клиенты
/// работали с одной и той же БД.
/// </summary>
public abstract class TestBase : IClassFixture<WebApplicationFactory<Program>>,
                                 IDisposable
{
    private readonly SqliteConnection _keepAlive;
    protected readonly WebApplicationFactory<Program> Factory;

    protected TestBase()
    {
        _keepAlive = new SqliteConnection("DataSource=:memory:");
        _keepAlive.Open();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.UseTestServer();

                builder.ConfigureServices(services =>
                {
                    var toRemove = services.Single(
                        d => d.ServiceType == typeof(DbContextOptions<AutoparkDbContext>));
                    services.Remove(toRemove);

                    services.AddDbContext<AutoparkDbContext>(o =>
                        o.UseSqlite(_keepAlive));

                    using var scope = services.BuildServiceProvider().CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AutoparkDbContext>();
                    db.Database.EnsureCreated();

                    // db.Enterprises.Add(new Enterprise { Id = 1, Name = "Demo" });
                    // db.SaveChanges();
                });
            });

        Console.WriteLine("TestBase: WebApplicationFactory и общая SQLite-БД инициализированы");
    }

    protected AutoparkDbContext CreateDbContext()
        => Factory.Services.CreateScope()
                 .ServiceProvider
                 .GetRequiredService<AutoparkDbContext>();

    protected HttpClient CreateHttpClient()
        => Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });

    public void Dispose() => _keepAlive.Dispose();
}