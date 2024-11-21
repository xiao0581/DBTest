using Neo4j.Driver;
using StackExchange.Redis;
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
        private static ConnectionMultiplexer GetConnection()
        {
            
            return ConnectionMultiplexer.Connect("localhost:6379");
        }

        private static List<User> LoadUsersFromJson(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonData);
        }

       
        public static async Task InsertSingleRecordAsync()
        {
            var connection = GetConnection();
            var db = connection.GetDatabase(); 

            Stopwatch stopwatch = Stopwatch.StartNew();
            await db.StringSetAsync("User:1000", JsonSerializer.Serialize(new User { UserId = 1000, Name = "Alice", Email = "alice@example.com" }));
            stopwatch.Stop();

            Console.WriteLine($"Redis: Single Insert - {stopwatch.ElapsedMilliseconds} ms");

            connection.Close();
        }

       
        public static async Task InsertBatchRecordsAsync()
        {
            var connection = GetConnection();
            var db = connection.GetDatabase();
            var users = LoadUsersFromJson("users.json");

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var user in users)
            {
                await db.StringSetAsync($"User:{user.UserId}", JsonSerializer.Serialize(user));
            }
            stopwatch.Stop();
            Console.WriteLine($"Redis: Batch Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");

            connection.Close();
        }

        
        public static async Task QuerySingleRecordAsync()
        {
            var connection = GetConnection();
            var db = connection.GetDatabase();

            Stopwatch stopwatch = Stopwatch.StartNew();
            var value = await db.StringGetAsync("User:1000");
            stopwatch.Stop();

            if (value.HasValue)
            {
                var user = JsonSerializer.Deserialize<User>(value);
                Console.WriteLine($"Redis: Single Query - {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Query Result: UserId=1000, Name={user.Name}, Email={user.Email}");
            }
            else
            {
                Console.WriteLine("No results found for UserId=1000.");
            }

            connection.Close();
        }

      
        public static async Task ClearAllDataAsync()
        {
            var connection = GetConnection();
            var db = connection.GetDatabase();

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
              
                await db.ExecuteAsync("FLUSHALL");
                stopwatch.Stop();
                Console.WriteLine($"Redis: cleared successfully in {stopwatch.ElapsedMilliseconds} ms");
            }
            finally
            {
                connection.Close();
            }
        }

    
        public static async Task ConcurrentInsertAsync()
        {
            var connection = GetConnection();
            var db = connection.GetDatabase();
            var users = LoadUsersFromJson("users.json");
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var user in users)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await db.StringSetAsync($"User:{user.UserId}", JsonSerializer.Serialize(user));
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Redis: Concurrent Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");

            connection.Close();
        }

      
        public static async Task ConcurrentQueryAsync()
        {
            var connection = GetConnection();
            var db = connection.GetDatabase();
            var random = new Random();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var userId = random.Next(1, 101);
                    var value = await db.StringGetAsync($"User:{userId}");

                    if (value.HasValue)
                    {
                        var user = JsonSerializer.Deserialize<User>(value);
                        Console.WriteLine($"Query Result: UserId={userId}, Name={user.Name}, Email={user.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"Query for UserId={userId} returned no results.");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Redis: Concurrent Query Completed in {stopwatch.ElapsedMilliseconds} ms");

            connection.Close();
        }
    }
}
