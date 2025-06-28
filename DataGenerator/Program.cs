using Autopark.DataGenerator.Models;
using Autopark.DataGenerator.Services;
using CommandLine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Autopark.DataGenerator;

class Program
{
    static async Task<int> Main(string[] args)
    {
        return await Parser.Default.ParseArguments<GeneratorOptions>(args)
            .MapResult(
                async (GeneratorOptions opts) =>
                {
                    try
                    {
                        await RunGenerator(opts);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                        return 1;
                    }
                },
                errors => Task.FromResult(1)
            );
    }

    static async Task RunGenerator(GeneratorOptions options)
    {
        Console.WriteLine("🚗 Генератор тестовых данных для Autopark");
        Console.WriteLine("========================================");
        Console.WriteLine($"Количество предприятий: {options.EnterpriseCount}");
        Console.WriteLine($"Машин на предприятие: {options.VehiclesPerEnterprise}");
        Console.WriteLine($"Доля водителей: {options.DriversRatio:P0}");
        Console.WriteLine($"Доля активных водителей: {options.ActiveDriversRatio:P0}");
        Console.WriteLine($"Дополнительных пользователей: {options.AdditionalUsers}");
        Console.WriteLine($"Seed: {options.Seed}");
        Console.WriteLine($"Файл вывода: {options.OutputFile}");
        if (options.ToDatabase)
        {
            Console.WriteLine($"Импорт в БД: Да");
            if (options.Truncate)
                Console.WriteLine($"Очистка таблиц: Да");
        }
        Console.WriteLine();

        // Создаем сервисы
        var services = new ServiceCollection();

        // Добавляем MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Autopark.UseCases.BrandModel.Commands.Create.CreateBrandModelCommand).Assembly));

        // Добавляем Infrastructure если нужен импорт в БД
        if (options.ToDatabase)
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                Console.WriteLine("Ошибка: Не указана строка подключения (--connection)");
                return;
            }

            services.AddInfrastructure(options.ConnectionString);
        }

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var generator = new DataGeneratorService(options.Seed, mediator);

        Console.WriteLine("⏳ Генерируем данные...");
        var data = generator.GenerateData(options);

        Console.WriteLine("💾 Сохраняем в файл...");
        generator.SaveToFile(data, options.OutputFile);

        if (options.ToDatabase)
        {
            Console.WriteLine("🗄️  Импортируем данные в базу данных...");
            await generator.ImportToDatabaseAsync(data, options);
        }

        var totalVehicles = data.Enterprises.Sum(e => e.Vehicles.Count);
        var totalDrivers = data.Enterprises.Sum(e => e.Drivers.Count);
        var vehiclesWithDrivers = data.Enterprises.Sum(e => e.Vehicles.Count(v => v.ActiveDriverId != null));

        Console.WriteLine();
        Console.WriteLine("✅ Генерация завершена!");
        Console.WriteLine($"📊 Статистика:");
        Console.WriteLine($"   Предприятий: {data.Enterprises.Count}");
        Console.WriteLine($"   Машин: {totalVehicles}");
        Console.WriteLine($"   Водителей: {totalDrivers}");
        Console.WriteLine($"   Машин с водителями: {vehiclesWithDrivers}");
        Console.WriteLine($"   Брендов/моделей: {data.BrandModels.Count}");
        Console.WriteLine($"   Пользователей: {data.Users.Count}");
        Console.WriteLine($"📁 Файл сохранен: {Path.GetFullPath(options.OutputFile)}");
    }
}