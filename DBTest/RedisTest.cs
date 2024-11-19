using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTest
{
    public class RedisTest
    {
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        public static void InsertSingleRecord()
        {
            var db = redis.GetDatabase();

            Stopwatch stopwatch = Stopwatch.StartNew();
            db.HashSet("User:1", new HashEntry[]
            {
            new HashEntry("UserId", 1),
            new HashEntry("Name", "Alice"),
            new HashEntry("Email", "alice@example.com")
            });
            stopwatch.Stop();

            Console.WriteLine($"Redis: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void InsertBatchRecords()
        {
            var db = redis.GetDatabase();
            Stopwatch stopwatch = Stopwatch.StartNew();

            db.HashSet("User:2", new HashEntry[]
            {
            new HashEntry("UserId", 2),
            new HashEntry("Name", "Bob"),
            new HashEntry("Email", "bob@example.com")
            });

            db.HashSet("User:3", new HashEntry[]
            {
            new HashEntry("UserId", 3),
            new HashEntry("Name", "Charlie"),
            new HashEntry("Email", "charlie@example.com")
            });

            stopwatch.Stop();
            Console.WriteLine($"Redis: Batch Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void QuerySingleRecord()
        {
            var db = redis.GetDatabase();

            Stopwatch stopwatch = Stopwatch.StartNew();
            var user = db.HashGetAll("User:1");
            stopwatch.Stop();

            Console.WriteLine($"Redis: Single Query - {stopwatch.ElapsedMilliseconds} ms");
            foreach (var field in user)
            {
                Console.WriteLine($"{field.Name}: {field.Value}");
            }
        }

        public static void QueryComplexJoin()
        {
            Console.WriteLine("Redis: Complex Query (Join) not supported");
        }

        public static void ConcurrentReadWrite()
        {
            var db = redis.GetDatabase();
            var tasks = new List<Task>();
            var random = new Random();

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var userId = i + 10;
                var userKey = $"User:{userId}";

                // Concurrent Write
                tasks.Add(Task.Run(() =>
                {
                    db.HashSet(userKey, new HashEntry[]
                    {
                    new HashEntry("UserId", userId),
                    new HashEntry("Name", $"User{userId}"),
                    new HashEntry("Email", $"user{userId}@example.com")
                    });
                }));

                // Concurrent Read
                tasks.Add(Task.Run(() =>
                {
                    db.HashGetAll(userKey);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            Console.WriteLine($"Redis: Concurrent Read/Write Test Completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
