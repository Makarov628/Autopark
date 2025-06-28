using Autopark.DataGenerator.Models;
using Autopark.Domain.BrandModel.Enums;
using Bogus;
using System.Text.Json;
using MediatR;
using Autopark.UseCases.BrandModel.Commands.Create;
using Autopark.UseCases.Enterprise.Commands.Create;
using Autopark.UseCases.Vehicle.Commands.Create;
using Autopark.UseCases.User.Commands.Create;
using Autopark.UseCases.User.Commands.Activate;
using Autopark.UseCases.User.Queries.GetActivationToken;
using Autopark.UseCases.Driver.Commands.Create;
using Autopark.UseCases.Driver.Commands.Update;
using Autopark.UseCases.Driver.Queries.GetAll;
using Autopark.UseCases.Vehicle.Commands.Update;
using Autopark.UseCases.Manager.Commands.Create;
using LanguageExt;

namespace Autopark.DataGenerator.Services;

public class DataGeneratorService
{
    private readonly Faker _faker;
    private readonly List<GeneratedBrandModel> _brandModels;
    private readonly IMediator _mediator;

    public DataGeneratorService(int seed, IMediator mediator)
    {
        _faker = new Faker("ru");
        _faker.Random = new Randomizer(seed);
        _mediator = mediator;

        _brandModels = GenerateBrandModels();
    }

    public GeneratedData GenerateData(GeneratorOptions options)
    {
        var brandModels = GenerateBrandModels();
        var users = GenerateUsers(options);
        var enterprises = GenerateEnterprises(options, users, brandModels);
        var managers = GenerateManagers(enterprises, users, options);

        return new GeneratedData
        {
            BrandModels = brandModels,
            Users = users,
            Enterprises = enterprises,
            Managers = managers
        };
    }

    private List<GeneratedBrandModel> GenerateBrandModels()
    {
        var carBrands = new[]
        {
            ("Toyota", "Camry", TransportType.Car, FuelType.Gasoline, 5, 500),
            ("Toyota", "Corolla", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Honda", "Civic", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Honda", "Accord", TransportType.Car, FuelType.Gasoline, 5, 500),
            ("BMW", "X5", TransportType.Car, FuelType.Gasoline, 5, 600),
            ("BMW", "3 Series", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Mercedes", "C-Class", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Mercedes", "E-Class", TransportType.Car, FuelType.Gasoline, 5, 500),
            ("Audi", "A4", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Audi", "Q5", TransportType.Car, FuelType.Gasoline, 5, 550),
            ("Volkswagen", "Golf", TransportType.Car, FuelType.Gasoline, 5, 400),
            ("Volkswagen", "Passat", TransportType.Car, FuelType.Gasoline, 5, 500),
            ("Tesla", "Model 3", TransportType.Car, FuelType.Electricity, 5, 450),
            ("Tesla", "Model S", TransportType.Car, FuelType.Electricity, 5, 500),
            ("Nissan", "Leaf", TransportType.Car, FuelType.Electricity, 5, 400),
            ("Ford", "Focus", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Chevrolet", "Cruze", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Hyundai", "Elantra", TransportType.Car, FuelType.Gasoline, 5, 450),
            ("Kia", "Rio", TransportType.Car, FuelType.Gasoline, 5, 400),
            ("Mazda", "3", TransportType.Car, FuelType.Gasoline, 5, 450)
        };

        var truckBrands = new[]
        {
            ("Volvo", "FH16", TransportType.Truck, FuelType.Diesel, 2, 20000),
            ("Scania", "R500", TransportType.Truck, FuelType.Diesel, 2, 18000),
            ("MAN", "TGX", TransportType.Truck, FuelType.Diesel, 2, 16000),
            ("Mercedes", "Actros", TransportType.Truck, FuelType.Diesel, 2, 18000),
            ("Iveco", "Stralis", TransportType.Truck, FuelType.Diesel, 2, 15000),
            ("DAF", "XF", TransportType.Truck, FuelType.Diesel, 2, 16000),
            ("Renault", "T", TransportType.Truck, FuelType.Diesel, 2, 16000),
            ("Isuzu", "Forward", TransportType.Truck, FuelType.Diesel, 2, 8000),
            ("Hino", "500", TransportType.Truck, FuelType.Diesel, 2, 10000),
            ("Ural", "4320", TransportType.Truck, FuelType.Diesel, 2, 12000)
        };

        var busBrands = new[]
        {
            ("Mercedes", "Sprinter", TransportType.Bus, FuelType.Diesel, 20, 3000),
            ("Volkswagen", "Crafter", TransportType.Bus, FuelType.Diesel, 18, 2800),
            ("Ford", "Transit", TransportType.Bus, FuelType.Diesel, 16, 2500),
            ("Iveco", "Daily", TransportType.Bus, FuelType.Diesel, 22, 3500),
            ("Fiat", "Ducato", TransportType.Bus, FuelType.Diesel, 16, 2500),
            ("Peugeot", "Boxer", TransportType.Bus, FuelType.Diesel, 16, 2500),
            ("Citroen", "Jumper", TransportType.Bus, FuelType.Diesel, 16, 2500),
            ("Renault", "Master", TransportType.Bus, FuelType.Diesel, 18, 2800),
            ("Nissan", "NV300", TransportType.Bus, FuelType.Diesel, 16, 2500),
            ("Opel", "Movano", TransportType.Bus, FuelType.Diesel, 16, 2500)
        };

        var allBrands = carBrands.Concat(truckBrands).Concat(busBrands).ToList();
        var brandModels = new List<GeneratedBrandModel>();

        for (int i = 0; i < allBrands.Count; i++)
        {
            var (brand, model, transportType, fuelType, seats, capacity) = allBrands[i];
            brandModels.Add(new GeneratedBrandModel
            {
                Id = Guid.NewGuid().ToString(),
                BrandName = brand,
                ModelName = model,
                TransportType = transportType.ToString(),
                FuelType = fuelType.ToString(),
                SeatsNumber = (uint)seats,
                MaximumLoadCapacityInKillograms = (uint)capacity
            });
        }

        return brandModels;
    }

    private List<GeneratedUser> GenerateUsers(GeneratorOptions options)
    {
        var totalVehicles = options.EnterpriseCount * options.VehiclesPerEnterprise;
        var totalDrivers = (int)(totalVehicles * options.DriversRatio);
        var totalManagers = options.Managers;
        var totalUsers = totalDrivers + totalManagers + options.AdditionalUsers;

        var users = new List<GeneratedUser>();
        for (int i = 0; i < totalUsers; i++)
        {
            var user = new GeneratedUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = _faker.Internet.Email(),
                FirstName = _faker.Name.FirstName(),
                LastName = _faker.Name.LastName(),
                DateOfBirth = _faker.Date.Between(DateTime.Now.AddYears(-65), DateTime.Now.AddYears(-18))
            };
            users.Add(user);
        }

        return users;
    }

    private List<GeneratedEnterprise> GenerateEnterprises(GeneratorOptions options, List<GeneratedUser> users, List<GeneratedBrandModel> brandModels)
    {
        var enterpriseFaker = new Faker<GeneratedEnterprise>("ru")
            .RuleFor(e => e.Id, f => Guid.NewGuid().ToString())
            .RuleFor(e => e.Name, f => f.Company.CompanyName())
            .RuleFor(e => e.Address, f => f.Address.FullAddress());

        var enterprises = enterpriseFaker.Generate(options.EnterpriseCount);
        var userIndex = 0;

        for (int i = 0; i < enterprises.Count; i++)
        {
            enterprises[i].Vehicles = GenerateVehicles(options.VehiclesPerEnterprise, enterprises[i].Id, brandModels);
            var updatedEnterprise = GenerateDrivers(enterprises[i], options, users, ref userIndex);
            enterprises[i] = updatedEnterprise;
        }

        return enterprises;
    }

    private List<GeneratedVehicle> GenerateVehicles(int count, string enterpriseId, List<GeneratedBrandModel> brandModels)
    {
        var vehicleFaker = new Faker<GeneratedVehicle>("ru")
            .RuleFor(v => v.Id, f => Guid.NewGuid().ToString())
            .RuleFor(v => v.Name, f => f.Commerce.ProductName())
            .RuleFor(v => v.Price, f => f.Random.Decimal(500000, 5000000))
            .RuleFor(v => v.MileageInKilometers, f => f.Random.Int(0, 200000))
            .RuleFor(v => v.Color, f => f.PickRandom("Белый", "Черный", "Серебристый", "Красный", "Синий", "Зеленый", "Желтый", "Оранжевый", "Серый", "Коричневый"))
            .RuleFor(v => v.RegistrationNumber, f => GenerateRegistrationNumber())
            .RuleFor(v => v.BrandModelId, f => f.PickRandom(brandModels).Id);

        return vehicleFaker.Generate(count);
    }

    private List<GeneratedVehicle> GenerateVehicles(List<GeneratedEnterprise> enterprises, GeneratorOptions options)
    {
        var vehicleFaker = new Faker<GeneratedVehicle>("ru")
            .RuleFor(v => v.Id, f => Guid.NewGuid().ToString())
            .RuleFor(v => v.Name, f => f.Commerce.ProductName())
            .RuleFor(v => v.Price, f => f.Random.Decimal(500000, 5000000))
            .RuleFor(v => v.MileageInKilometers, f => f.Random.Int(0, 200000))
            .RuleFor(v => v.Color, f => f.PickRandom("Белый", "Черный", "Серебристый", "Красный", "Синий", "Зеленый", "Желтый", "Оранжевый", "Серый", "Коричневый"))
            .RuleFor(v => v.RegistrationNumber, f => GenerateRegistrationNumber())
            .RuleFor(v => v.BrandModelId, f => f.PickRandom(enterprises.SelectMany(e => e.Vehicles)).BrandModelId);

        return vehicleFaker.Generate(options.EnterpriseCount * options.VehiclesPerEnterprise);
    }

    private GeneratedEnterprise GenerateDrivers(GeneratedEnterprise enterprise, GeneratorOptions options, List<GeneratedUser> users, ref int userIndex)
    {
        var totalDriversForEnterprise = (int)(enterprise.Vehicles.Count * options.DriversRatio);
        var activeDriversCount = (int)Math.Ceiling(enterprise.Vehicles.Count * options.ActiveDriversRatio);
        var drivers = new List<GeneratedDriver>();

        // Создаем всех водителей для предприятия
        for (int i = 0; i < totalDriversForEnterprise && userIndex < users.Count; i++)
        {
            var driver = new GeneratedDriver
            {
                Id = Guid.NewGuid().ToString(),
                UserId = users[userIndex].Id,
                Salary = _faker.Random.Decimal(50000, 150000),
                VehicleId = null // Пока не привязываем к машине
            };

            drivers.Add(driver);
            userIndex++;
        }

        // Привязываем активных водителей к машинам
        for (int i = 0; i < activeDriversCount && i < drivers.Count && i < enterprise.Vehicles.Count; i++)
        {
            drivers[i].VehicleId = enterprise.Vehicles[i].Id;
            enterprise.Vehicles[i].ActiveDriverId = drivers[i].Id;
        }

        enterprise.Drivers = drivers;
        return enterprise;
    }

    private string GenerateRegistrationNumber()
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        var letter1 = _faker.PickRandom(letters);
        var number1 = _faker.Random.Int(0, 9);
        var number2 = _faker.Random.Int(0, 9);
        var number3 = _faker.Random.Int(0, 9);
        var letter2 = _faker.PickRandom(letters);
        var letter3 = _faker.PickRandom(letters);
        var region = _faker.Random.Int(1, 999);

        return $"{letter1}{number1}{number2}{number3}{letter2}{letter3}{region:D3}";
    }

    public void SaveToFile(GeneratedData data, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }

    public async Task ImportToDatabaseAsync(GeneratedData data, GeneratorOptions options)
    {
        Console.WriteLine("🗄️  Импортируем данные в базу данных...");

        if (options.Truncate)
        {
            Console.WriteLine("⚠️  Очистка таблиц не поддерживается через UseCases. Используйте прямые SQL-запросы.");
        }

        var brandModelIdMap = new Dictionary<string, int>();
        var userIdMap = new Dictionary<string, int>();
        var enterpriseIdMap = new Dictionary<string, int>();
        var vehicleIdMap = new Dictionary<string, int>();

        // 1. Импорт BrandModels
        Console.WriteLine("📦 Импортируем бренды и модели...");
        foreach (var brandModel in data.BrandModels)
        {
            var command = new CreateBrandModelCommand(
                brandModel.BrandName,
                brandModel.ModelName,
                Enum.Parse<TransportType>(brandModel.TransportType),
                Enum.Parse<FuelType>(brandModel.FuelType),
                brandModel.SeatsNumber,
                brandModel.MaximumLoadCapacityInKillograms
            );

            var result = await _mediator.Send(command);
            if (result.IsFail)
            {
                Console.WriteLine($"❌ Ошибка создания BrandModel {brandModel.BrandName} {brandModel.ModelName}: {result}");
                continue;
            }

            var newId = brandModelIdMap.Count + 1;
            brandModelIdMap[brandModel.Id] = newId;
            Console.WriteLine($"✅ BrandModel '{brandModel.BrandName} {brandModel.ModelName}' создан с ID {newId}");
        }

        // 2. Импорт Users
        Console.WriteLine("👥 Импортируем пользователей...");
        foreach (var user in data.Users)
        {
            var command = new CreateUserCommand
            {
                Email = user.Email,
                Phone = user.Phone,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };

            var userId = await _mediator.Send(command);
            userIdMap[user.Id] = userId;
            Console.WriteLine($"✅ Пользователь {user.Email} создан с ID {userId}");
        }

        // 3. Активируем пользователей
        Console.WriteLine("🔐 Активируем пользователей...");
        foreach (var user in data.Users)
        {
            // Получаем токен активации
            var tokenQuery = new GetActivationTokenQuery { Email = user.Email };
            var tokenResponse = await _mediator.Send(tokenQuery);

            if (!tokenResponse.Success)
            {
                Console.WriteLine($"⚠️ Не удалось получить токен для пользователя {user.Email}");
                continue;
            }

            // Активируем пользователя
            var activateCommand = new ActivateUserCommand
            {
                Token = tokenResponse.Token,
                Password = "Password123!", // Временный пароль
                RepeatPassword = "Password123!"
            };

            var activateResult = await _mediator.Send(activateCommand);
            if (activateResult)
            {
                Console.WriteLine($"✅ Пользователь {user.Email} активирован");
            }
            else
            {
                Console.WriteLine($"❌ Ошибка активации пользователя {user.Email}");
            }
        }

        // 4. Импорт Enterprises
        Console.WriteLine("🏢 Импортируем предприятия...");
        foreach (var enterprise in data.Enterprises)
        {
            var command = new CreateEnterpriseCommand(
                enterprise.Name,
                enterprise.Address
            );

            var result = await _mediator.Send(command);
            if (result.IsFail)
            {
                Console.WriteLine($"❌ Ошибка создания Enterprise {enterprise.Name}: {result}");
                continue;
            }

            var newId = enterpriseIdMap.Count + 1;
            enterpriseIdMap[enterprise.Id] = newId;
            Console.WriteLine($"✅ Предприятие '{enterprise.Name}' создано с ID {newId}");
        }

        // 5. Импорт Vehicles
        Console.WriteLine("🚗 Импортируем машины...");
        var createdVehiclesCount = 0;
        foreach (var enterprise in data.Enterprises)
        {
            if (!enterpriseIdMap.TryGetValue(enterprise.Id, out var enterpriseId))
            {
                Console.WriteLine($"⚠️ Предприятие {enterprise.Id} не найдено в базе");
                continue;
            }

            Console.WriteLine($"🏢 Обрабатываем предприятие '{enterprise.Name}' (ID: {enterpriseId})");
            Console.WriteLine($"   Машин для импорта: {enterprise.Vehicles.Count}");

            foreach (var vehicle in enterprise.Vehicles)
            {
                if (!brandModelIdMap.TryGetValue(vehicle.BrandModelId, out var brandModelId))
                {
                    Console.WriteLine($"⚠️ BrandModel {vehicle.BrandModelId} не найден в базе");
                    continue;
                }

                Console.WriteLine($"   Создаем машину '{vehicle.Name}' с BrandModel ID {brandModelId}");

                var command = new CreateVehicleCommand(
                    vehicle.Name,
                    vehicle.Price,
                    vehicle.MileageInKilometers,
                    vehicle.Color,
                    vehicle.RegistrationNumber,
                    brandModelId,
                    enterpriseId
                );

                var result = await _mediator.Send(command);
                if (result.IsFail)
                {
                    Console.WriteLine($"❌ Ошибка создания Vehicle {vehicle.Name}: {result}");
                    continue;
                }

                var newId = vehicleIdMap.Count + 1;
                vehicleIdMap[vehicle.Id] = newId;
                createdVehiclesCount++;
                Console.WriteLine($"✅ Машина '{vehicle.Name}' создана с ID {newId} (всего: {createdVehiclesCount})");
            }
        }

        Console.WriteLine($"✅ Импорт машин завершен. Создано: {createdVehiclesCount}");

        // 6. Импорт Drivers
        Console.WriteLine("👨‍💼 Импортируем водителей...");
        var createdDriversCount = 0;
        foreach (var enterprise in data.Enterprises)
        {
            if (!enterpriseIdMap.TryGetValue(enterprise.Id, out var enterpriseId))
            {
                Console.WriteLine($"⚠️ Предприятие {enterprise.Id} не найдено в базе");
                continue;
            }

            Console.WriteLine($"🏢 Обрабатываем предприятие '{enterprise.Name}' (ID: {enterpriseId})");
            Console.WriteLine($"   Водителей для импорта: {enterprise.Drivers.Count}");

            foreach (var driver in enterprise.Drivers)
            {
                if (!userIdMap.TryGetValue(driver.UserId, out var userId))
                {
                    Console.WriteLine($"⚠️ Пользователь {driver.UserId} не найден в базе");
                    continue;
                }

                Console.WriteLine($"   Создаем водителя для пользователя ID {userId} с зарплатой {driver.Salary}");

                var command = new CreateDriverCommand(
                    userId,
                    driver.Salary,
                    enterpriseId
                );

                var result = await _mediator.Send(command);
                if (result.IsFail)
                {
                    Console.WriteLine($"❌ Ошибка создания Driver: {result}");
                    continue;
                }

                createdDriversCount++;
                Console.WriteLine($"✅ Водитель создан (всего: {createdDriversCount})");

                // Привязываем водителя к машине, если есть
                if (driver.VehicleId != null && vehicleIdMap.TryGetValue(driver.VehicleId, out var vehicleId))
                {
                    Console.WriteLine($"🔗 Водитель привязан к машине {vehicleId}");
                }
            }
        }

        Console.WriteLine($"✅ Импорт водителей завершен. Создано: {createdDriversCount}");

        // 6.5. Получаем список всех водителей для создания маппинга
        Console.WriteLine("🔍 Получаем список водителей для создания маппинга...");
        var getAllDriversQuery = new GetAllDriversQuery();
        var driversResult = await _mediator.Send(getAllDriversQuery);

        if (driversResult.IsFail)
        {
            Console.WriteLine($"❌ Ошибка получения списка водителей: {driversResult}");
            return;
        }

        var drivers = driversResult.Match(
            success => success,
            failure => new List<DriversResponse>()
        );
        var driverIdMap = new Dictionary<string, int>();

        // Создаем маппинг по UserId и EnterpriseId
        foreach (var enterprise in data.Enterprises)
        {
            if (!enterpriseIdMap.TryGetValue(enterprise.Id, out var enterpriseId))
                continue;

            foreach (var driver in enterprise.Drivers)
            {
                if (!userIdMap.TryGetValue(driver.UserId, out var userId))
                    continue;

                var dbDriver = drivers.FirstOrDefault(d => d.UserId == userId && d.EnterpriseId == enterpriseId);
                if (dbDriver != null)
                {
                    driverIdMap[driver.Id] = dbDriver.Id;
                    Console.WriteLine($"🔗 Маппинг: водитель {driver.Id} -> ID {dbDriver.Id} (пользователь {userId})");
                }
            }
        }

        // 7. Обновляем водителей, привязывая их к машинам
        Console.WriteLine("🔗 Обновляем водителей с привязкой к машинам...");
        var updatedDriversCount = 0;
        foreach (var enterprise in data.Enterprises)
        {
            if (!enterpriseIdMap.TryGetValue(enterprise.Id, out var enterpriseId))
                continue;

            foreach (var driver in enterprise.Drivers)
            {
                if (driver.VehicleId != null &&
                    driverIdMap.TryGetValue(driver.Id, out var driverId) &&
                    vehicleIdMap.TryGetValue(driver.VehicleId, out var vehicleId))
                {
                    var updateDriverCommand = new UpdateDriverCommand(
                        driverId,
                        driver.Salary,
                        enterpriseId,
                        vehicleId
                    );

                    var result = await _mediator.Send(updateDriverCommand);
                    if (result.IsFail)
                    {
                        Console.WriteLine($"❌ Ошибка обновления водителя {driverId}: {result}");
                        continue;
                    }

                    updatedDriversCount++;
                    Console.WriteLine($"✅ Водитель {driverId} привязан к машине {vehicleId}");
                }
            }
        }

        Console.WriteLine($"✅ Обновление водителей завершено. Обновлено: {updatedDriversCount}");

        // 8. Обновляем машины с ActiveDriverId
        Console.WriteLine("🔗 Обновляем машины с привязкой к водителям...");
        var updatedVehiclesCount = 0;
        foreach (var enterprise in data.Enterprises)
        {
            foreach (var vehicle in enterprise.Vehicles)
            {
                if (vehicle.ActiveDriverId != null &&
                    driverIdMap.TryGetValue(vehicle.ActiveDriverId, out var driverId) &&
                    vehicleIdMap.TryGetValue(vehicle.Id, out var vehicleId))
                {
                    // Получаем текущие данные машины
                    var currentVehicle = enterprise.Vehicles.First(v => v.Id == vehicle.Id);

                    var updateCommand = new UpdateVehicleCommand(
                        vehicleId,
                        currentVehicle.Name,
                        currentVehicle.Price,
                        currentVehicle.MileageInKilometers,
                        currentVehicle.Color,
                        currentVehicle.RegistrationNumber,
                        brandModelIdMap[currentVehicle.BrandModelId],
                        enterpriseIdMap[enterprise.Id],
                        driverId
                    );

                    var result = await _mediator.Send(updateCommand);
                    if (result.IsFail)
                    {
                        Console.WriteLine($"❌ Ошибка обновления машины {vehicleId}: {result}");
                        continue;
                    }

                    updatedVehiclesCount++;
                    Console.WriteLine($"✅ Машина {vehicleId} обновлена с водителем {driverId}");

                    // Небольшая задержка между обновлениями, чтобы избежать конфликтов Entity Framework
                    await Task.Delay(100);
                }
            }
        }

        Console.WriteLine($"✅ Обновление машин завершено. Обновлено: {updatedVehiclesCount}");

        // 9. Импортируем менеджеров
        Console.WriteLine("👨‍💼 Импортируем менеджеров...");
        var managerIdMap = new Dictionary<string, int>();

        foreach (var manager in data.Managers)
        {
            if (userIdMap.TryGetValue(manager.UserId, out var userId))
            {
                var enterpriseDbIds = manager.EnterpriseIds
                    .Where(eid => enterpriseIdMap.ContainsKey(eid))
                    .Select(eid => enterpriseIdMap[eid])
                    .ToList();

                var createCommand = new CreateManagerCommand(userId, enterpriseDbIds);
                var result = await _mediator.Send(createCommand);
                if (result.IsFail)
                {
                    Console.WriteLine($"❌ Ошибка создания менеджера для пользователя {userId}: {result}");
                    continue;
                }

                var managerId = result.Match(
                    success => success,
                    error => 0
                );

                if (managerId > 0)
                {
                    managerIdMap[manager.Id] = managerId;
                    Console.WriteLine($"✅ Менеджер создан с ID {managerId} для пользователя {userId}, предприятий: {enterpriseDbIds.Count}");
                }
            }
        }

        Console.WriteLine($"✅ Импорт менеджеров завершен. Создано: {managerIdMap.Count}");

        var totalVehicles = data.Enterprises.Sum(e => e.Vehicles.Count);
        var totalDrivers = data.Enterprises.Sum(e => e.Drivers.Count);
        var vehiclesWithDrivers = data.Enterprises.Sum(e => e.Vehicles.Count(v => v.ActiveDriverId != null));
        var totalManagers = data.Managers.Count;

        Console.WriteLine($"✅ Генерация завершена!");
        Console.WriteLine($"📊 Статистика:");
        Console.WriteLine($"   Предприятий: {data.Enterprises.Count}");
        Console.WriteLine($"   Машин: {totalVehicles}");
        Console.WriteLine($"   Водителей: {totalDrivers}");
        Console.WriteLine($"   Машин с водителями: {vehiclesWithDrivers}");
        Console.WriteLine($"   Менеджеров: {totalManagers}");
        Console.WriteLine($"   Брендов/моделей: {data.BrandModels.Count}");
        Console.WriteLine($"   Пользователей: {data.Users.Count}");
    }

    private List<GeneratedManager> GenerateManagers(List<GeneratedEnterprise> enterprises, List<GeneratedUser> users, GeneratorOptions options)
    {
        var managers = new List<GeneratedManager>();
        var enterpriseIds = enterprises.Select(e => e.Id).ToList();
        var totalVehicles = options.EnterpriseCount * options.VehiclesPerEnterprise;
        var totalDrivers = (int)(totalVehicles * options.DriversRatio);

        // Пользователи для менеджеров начинаются после водителей и дополнительных пользователей
        var managerUserStartIndex = totalDrivers + options.AdditionalUsers;
        var availableUserIds = users.Skip(managerUserStartIndex).Take(options.Managers).ToList();

        // Гарантируем, что у каждого предприятия будет хотя бы один менеджер
        var unassignedEnterprises = new System.Collections.Generic.HashSet<string>(enterpriseIds);
        var managerCount = Math.Min(options.Managers, availableUserIds.Count);

        for (int i = 0; i < managerCount; i++)
        {
            if (i >= availableUserIds.Count) break;

            var manager = new GeneratedManager
            {
                Id = Guid.NewGuid().ToString(),
                UserId = availableUserIds[i].Id
            };

            // Если есть неприсвоенные предприятия, обязательно назначаем хотя бы одно
            if (unassignedEnterprises.Count > 0)
            {
                var enterpriseId = unassignedEnterprises.First();
                manager.EnterpriseIds.Add(enterpriseId);
                unassignedEnterprises.Remove(enterpriseId);
            }

            // Добавляем дополнительные предприятия случайным образом
            var additionalEnterprises = _faker.Random.Int(0, Math.Min(options.EnterprisesPerManager - 1, enterpriseIds.Count - 1));
            var availableForAdditional = enterpriseIds.Where(id => !manager.EnterpriseIds.Contains(id)).ToList();

            for (int j = 0; j < additionalEnterprises && availableForAdditional.Count > 0; j++)
            {
                var randomIndex = _faker.Random.Int(0, availableForAdditional.Count - 1);
                var enterpriseId = availableForAdditional[randomIndex];
                manager.EnterpriseIds.Add(enterpriseId);
                availableForAdditional.RemoveAt(randomIndex);
            }

            managers.Add(manager);
        }

        // Если остались неприсвоенные предприятия, распределяем их между существующими менеджерами
        while (unassignedEnterprises.Count > 0 && managers.Count > 0)
        {
            var enterpriseId = unassignedEnterprises.First();
            var randomManager = managers[_faker.Random.Int(0, managers.Count - 1)];
            randomManager.EnterpriseIds.Add(enterpriseId);
            unassignedEnterprises.Remove(enterpriseId);
        }

        return managers;
    }
}