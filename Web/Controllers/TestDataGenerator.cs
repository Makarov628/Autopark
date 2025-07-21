using Octonica.ClickHouseClient;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class TestDataGenerator
{
    public static async Task GenerateTestTrack(ClickHouseConnection connection)
    {
        // Создаем таблицу, если её нет
        const string createTableSql = @"
            CREATE TABLE IF NOT EXISTS can_telemetry
            (
                vehicle_id UInt32,
                ts DateTime64(3),
                pos Point,
                speed Float32,
                rpm UInt16,
                fuel_lvl UInt8
            ) ENGINE = MergeTree()
            ORDER BY (vehicle_id, ts)";

        await connection.OpenAsync();
        await using var createCmd = connection.CreateCommand(createTableSql);
        await createCmd.ExecuteNonQueryAsync();

        // Очищаем старые данные для vehicle_id = 42
        const string deleteSql = "DELETE FROM can_telemetry WHERE vehicle_id = 42";
        await using var deleteCmd = connection.CreateCommand(deleteSql);
        await deleteCmd.ExecuteNonQueryAsync();

        // Создаем трек по кругу в центре Алматы
        var centerLat = 43.2567;
        var centerLon = 76.9286;
        var radius = 0.01; // ~1 км
        var startTime = DateTime.UtcNow.AddHours(-2); // 2 часа назад

        // Используем параметризованный запрос с point({lon:Float64}, {lat:Float64})
        const string insertSql = @"
            INSERT INTO can_telemetry (vehicle_id, ts, pos, speed, rpm, fuel_lvl)
            VALUES ({vid:UInt32}, {ts:DateTime64}, ({lon:Float64}, {lat:Float64}), {speed:Float32}, {rpm:UInt16}, {fuel:UInt8})";

        await using var insertCmd = connection.CreateCommand(insertSql);

        for (int i = 0; i < 100; i++)
        {
            var angle = (2 * Math.PI * i) / 100;
            var lat = centerLat + radius * Math.Cos(angle);
            var lon = centerLon + radius * Math.Sin(angle);
            var time = startTime.AddMinutes(i * 2); // каждые 2 минуты
            var speed = 30 + (i % 20); // 30-50 км/ч
            var rpm = (ushort)(1500 + (i % 500)); // 1500-2000 об/мин
            var fuel = (byte)(80 - (i / 10)); // постепенно уменьшается

            insertCmd.Parameters.Clear();
            insertCmd.Parameters.AddWithValue("vid", 42u);
            insertCmd.Parameters.AddWithValue("ts", time);
            insertCmd.Parameters.AddWithValue("lon", lon);
            insertCmd.Parameters.AddWithValue("lat", lat);
            insertCmd.Parameters.AddWithValue("speed", speed);
            insertCmd.Parameters.AddWithValue("rpm", rpm);
            insertCmd.Parameters.AddWithValue("fuel", fuel);

            await insertCmd.ExecuteNonQueryAsync();
        }

        Console.WriteLine($"Создан тестовый трек для vehicle_id = 42 с 100 точками");
    }
}