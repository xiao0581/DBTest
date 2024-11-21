﻿using Cassandra.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DBTest
{
    public class SqlServerTest
    {
        private static string connectionString = "Data Source=XIAO-PC\\XIAODATA;Integrated Security=True;Database=DBtest;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // 加载 JSON 数据
        public static async Task<List<User>> LoadUsersFromJsonAsync(string filePath)
        {
            var jsonData = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<List<User>>(jsonData);
        }

        // 单条记录插入
        public static async Task InsertSingleRecordAsync()
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
            var parameters = new { UserId = 1000, Name = "Alice", Email = "alice@example.com" };

            Stopwatch stopwatch = Stopwatch.StartNew();
            await connection.ExecuteAsync(query, parameters);
            stopwatch.Stop();

            Console.WriteLine($"SQL Server: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        // 批量插入
        public static async Task InsertBatchRecordsAsync()
        {
            var users = await LoadUsersFromJsonAsync("users.json");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";

            Stopwatch stopwatch = Stopwatch.StartNew();
            await connection.ExecuteAsync(query, users);
            stopwatch.Stop();

            Console.WriteLine($"SQL Server: Batch Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");
        }

        // 单条查询
        public static async Task QuerySingleRecordAsync()
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Users WHERE UserId = @UserId";
            var parameters = new { UserId = 1000 };

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = await connection.QuerySingleOrDefaultAsync(query, parameters);
            stopwatch.Stop();

            Console.WriteLine($"SQL Server: Single Query - {stopwatch.ElapsedMilliseconds} ms");
        }


        public static async Task ClearDatabaseAsync()
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // 使用 TRUNCATE 清空表
            var truncateQuery = "TRUNCATE TABLE Users;";
            await connection.ExecuteAsync(truncateQuery);

            Console.WriteLine("Database cleared successfully.");
        }
        // 并发插入
        public static async Task ConcurrentInsertAsync()
        {
            var users = await LoadUsersFromJsonAsync("users.json");
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (var user in users)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();

                    var insertQuery = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
                    await connection.ExecuteAsync(insertQuery, user);
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"SQL Server: Concurrent Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");
        }

        // 并发查询
        public static async Task ConcurrentQueryAsync()
        {
            var random = new Random();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();

                    var userId = random.Next(1, 101);
                    var query = "SELECT * FROM Users WHERE UserId = @UserId";
                    var parameters = new { UserId = userId };

                    var result = await connection.QuerySingleOrDefaultAsync(query, parameters);

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

            Console.WriteLine($"SQL Server: Concurrent Query Completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }


}

