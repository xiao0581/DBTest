using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DBTest
{
    public class RedisTest
    {
        private static IDriver GetDriver()
        {
            return GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));
        }

        private static List<User> LoadUsersFromJson(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonData);
        }

        public static async Task InsertSingleRecord()
        {
            using var driver = GetDriver();
            using var session = driver.AsyncSession();

            Stopwatch stopwatch = Stopwatch.StartNew();
            await session.RunAsync(
                "CREATE (u:User {UserId: $UserId, Name: $Name, Email: $Email})",
                new { UserId = 1000, Name = "Alice", Email = "alice@example.com" }
            );
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task InsertBatchRecords()
        {
            using var driver = GetDriver();
            using var session = driver.AsyncSession();
            var users = LoadUsersFromJson("users.json");

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var user in users)
            {
                await session.RunAsync(
                    "CREATE (u:User {UserId: $UserId, Name: $Name, Email: $Email})",
                    new { user.UserId, user.Name, user.Email }
                );
            }
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Batch Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task QuerySingleRecord()
        {
            using var driver = GetDriver();
            using var session = driver.AsyncSession();

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = await session.RunAsync(
                "MATCH (u:User {UserId: $UserId}) RETURN u.Name AS Name, u.Email AS Email",
                new { UserId = 1000 } // 假设查询 UserId 为 1
            );

            stopwatch.Stop();
            if (await result.FetchAsync())
            {
                var name = result.Current["Name"].As<string>();
                var email = result.Current["Email"].As<string>();
                Console.WriteLine($"Neo4j: Single Query - {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Query Result: UserId=1, Name={name}, Email={email}");
            }
            else
            {
                Console.WriteLine("No results found for UserId=1.");
            }
        }

        public static async Task ClearUsersAsync()
{
    using var driver = GetDriver();
    using var session = driver.AsyncSession();

    Stopwatch stopwatch = Stopwatch.StartNew();
    try
    {
        await session.RunAsync("MATCH (u:User) DETACH DELETE u");
        stopwatch.Stop();
        Console.WriteLine($"Neo4j: Cleared all User nodes - {stopwatch.ElapsedMilliseconds} ms");
    }
    finally
    {
        await session.CloseAsync();
        await driver.CloseAsync();
    }
}
        public static async Task ConcurrentInsert()
        {
            var driver = GetDriver();
            var users = LoadUsersFromJson("users.json");
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var user in users)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var session = driver.AsyncSession();
                    await session.RunAsync(
                        "CREATE (u:User {UserId: $UserId, Name: $Name, Email: $Email})",
                        new { user.UserId, user.Name, user.Email }
                    );
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Concurrent Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");

            await driver.CloseAsync();
        }


        public static async Task ConcurrentQuery()
        {
            var driver = GetDriver();
            var random = new Random();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var session = driver.AsyncSession();
                    var userId = random.Next(1, 101);
                    var result = await session.RunAsync(
                        "MATCH (u:User {UserId: $UserId}) RETURN u.Name AS Name, u.Email AS Email",
                        new { UserId = userId }
                    );

                    // 手动检查结果
                    if (await result.FetchAsync())
                    {
                        var name = result.Current["Name"].As<string>();
                        var email = result.Current["Email"].As<string>();
                        Console.WriteLine($"Query Result: UserId={userId}, Name={name}, Email={email}");
                    }
                    else
                    {
                        Console.WriteLine($"Query for UserId={userId} returned no results.");
                    }
                }));
            }


            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Concurrent Query Completed in {stopwatch.ElapsedMilliseconds} ms");

            await driver.CloseAsync();
        }
    }
}
