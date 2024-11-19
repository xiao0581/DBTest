using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DBTest
{
    public class MongoDBTest
    {
        private static IMongoCollection<BsonDocument> GetCollection()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("TestDB");
            return database.GetCollection<BsonDocument>("Users");
        }

        public static async Task InsertSingleRecord()
        {
            var collection = GetCollection();
            var document = new BsonDocument { { "UserId", 1 }, { "Name", "Alice" }, { "Email", "alice@example.com" } };

            Stopwatch stopwatch = Stopwatch.StartNew();
            await collection.InsertOneAsync(document);
            stopwatch.Stop();
            Console.WriteLine($"MongoDB: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task InsertBatchRecords()
        {
            var collection = GetCollection();
            var documents = new[]
            {
            new BsonDocument { { "UserId", 2 }, { "Name", "Bob" }, { "Email", "bob@example.com" } },
            new BsonDocument { { "UserId", 3 }, { "Name", "Charlie" }, { "Email", "charlie@example.com" } }
        };

            Stopwatch stopwatch = Stopwatch.StartNew();
            await collection.InsertManyAsync(documents);
            stopwatch.Stop();
            Console.WriteLine($"MongoDB: Batch Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task QuerySingleRecord()
        {
            var collection = GetCollection();
            var filter = Builders<BsonDocument>.Filter.Eq("UserId", 1);

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            stopwatch.Stop();
            Console.WriteLine($"MongoDB: Single Query - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task QueryComplexJoin()
        {
            Console.WriteLine("MongoDB: Complex Query (Join) not directly supported");
        }

        public static async Task ConcurrentReadWrite()
        {
            var collection = GetCollection();
            var tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                var userId = i + 10;
                var insertTask = collection.InsertOneAsync(new BsonDocument { { "UserId", userId }, { "Name", $"User{userId}" }, { "Email", $"user{userId}@example.com" } });
                tasks.Add(insertTask);

                var readTask = collection.Find(Builders<BsonDocument>.Filter.Eq("UserId", userId)).FirstOrDefaultAsync();
                tasks.Add(readTask);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"MongoDB: Concurrent Read/Write Test Completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
