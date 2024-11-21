using Cassandra;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace DBTest
{
    public class PostgreSQLTest
    {
        private static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=12345;Database=Test";

        // 加载 JSON 数据
        private static async Task<List<User>> LoadUsersFromJsonAsync(string filePath)
        {
            var jsonData = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonData);
        }

        // 单条记录插入
        public static async Task InsertSingleRecordAsync()
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
            var parameters = new { UserId = 1000, Name = "Alice", Email = "alice@example.com" };

            Stopwatch stopwatch = Stopwatch.StartNew();
            await connection.ExecuteAsync(query, parameters);
            stopwatch.Stop();

            Console.WriteLine($"PostgreSQL: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        // 批量插入
        public static async Task InsertBatchRecordsAsync()
        {
            var users = await LoadUsersFromJsonAsync("users.json"); // 异步加载 JSON 数据

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";

            Stopwatch stopwatch = Stopwatch.StartNew();
            await connection.ExecuteAsync(query, users);
            stopwatch.Stop();

            Console.WriteLine($"PostgreSQL: Batch Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");
        }

        // 单条查询
        public static async Task QuerySingleRecordAsync()
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Users WHERE UserId = @UserId";
            var parameters = new { UserId = 1000 };

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = await connection.QuerySingleOrDefaultAsync(query, parameters);
            stopwatch.Stop();

            Console.WriteLine($"PostgreSQL: Single Query - {stopwatch.ElapsedMilliseconds} ms");
        }
        public static async Task ClearUsersTableAsync()
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    // DELETE 表中所有记录
    var deleteQuery = "DELETE FROM Users;";
    await connection.ExecuteAsync(deleteQuery);

    Console.WriteLine("PostgreSQL: Users table cleared successfully.");
}
        // 并发插入
        public static async Task ConcurrentInsertAsync()
        {
            var users = await LoadUsersFromJsonAsync("users.json"); // 异步加载 JSON 数据
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (var user in users)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var connection = new NpgsqlConnection(connectionString);
                    await connection.OpenAsync();

                    var insertQuery = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
                    await connection.ExecuteAsync(insertQuery, user);
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"PostgreSQL: Concurrent Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");
        }

        // 并发查询
        public static async Task ConcurrentQueryAsync()
        {
            var random = new Random();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            // 假设随机查询 UserId 的范围为 1 到 100
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var connection = new NpgsqlConnection(connectionString);
                    await connection.OpenAsync();

                    var userId = random.Next(1, 101); // 随机生成 UserId
                    var query = "SELECT * FROM Users WHERE UserId = @UserId";
                    var parameters = new { UserId = userId };

                    var result = await connection.QuerySingleOrDefaultAsync(query, parameters);

                    // 可选调试输出
                    if (result != null)
                    {
                        Console.WriteLine($"Query Result: UserId={result.UserId}, Name={result.Name}, Email={result.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"Query for UserId={userId} returned no results.");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"PostgreSQL: Concurrent Query Completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }

}

