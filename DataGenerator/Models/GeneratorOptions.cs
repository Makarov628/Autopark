using CommandLine;

namespace Autopark.DataGenerator.Models;

public class GeneratorOptions
{
    [Option('e', "enterprises", Required = true, HelpText = "Количество предприятий для генерации")]
    public int EnterpriseCount { get; set; }

    [Option('v', "vehicles-per-enterprise", Required = true, HelpText = "Количество машин на предприятие")]
    public int VehiclesPerEnterprise { get; set; }

    [Option('d', "drivers-ratio", Required = false, Default = 0.5, HelpText = "Доля водителей от общего количества машин (0.0 - 1.0)")]
    public double DriversRatio { get; set; }

    [Option('a', "active-drivers-ratio", Required = false, Default = 0.3, HelpText = "Доля активных водителей от общего количества водителей (0.0 - 1.0)")]
    public double ActiveDriversRatio { get; set; }

    [Option('o', "output", Default = "generated_data.json", HelpText = "Путь к файлу для сохранения данных")]
    public string OutputFile { get; set; } = "generated_data.json";

    [Option('s', "seed", Default = 12345, HelpText = "Seed для генератора случайных чисел")]
    public int Seed { get; set; } = 12345;

    [Option("to-db", Default = false, HelpText = "Импортировать данные в базу данных")]
    public bool ToDatabase { get; set; }

    [Option("connection", HelpText = "Строка подключения к базе данных (SqlServer)")]
    public string? ConnectionString { get; set; }

    [Option("truncate", Default = false, HelpText = "Очистить таблицы перед импортом в БД")]
    public bool Truncate { get; set; }

    [Option("additional-users", Default = 0, HelpText = "Количество дополнительных пользователей (не водителей)")]
    public int AdditionalUsers { get; set; }

    [Option('m', "managers", Required = false, Default = 5, HelpText = "Количество менеджеров")]
    public int Managers { get; set; }

    [Option('e', "enterprises-per-manager", Required = false, Default = 2, HelpText = "Среднее количество предприятий на менеджера")]
    public int EnterprisesPerManager { get; set; }
}