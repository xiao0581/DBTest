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

        // 加载 JSON 数据
        private static List<BsonDocument> LoadUsersFromJson(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            var users = JsonSerializer.Deserialize<List<User>>(jsonData);

            return users.Select(u =>
                new BsonDocument
                {
                    { "UserId", u.UserId },
                    { "Name", u.Name },
                    { "Email", u.Email }
                }).ToList();
        }

        // 单条记录插入
        public static async Task InsertSingleRecord()
        {
            var collection = GetCollection();
            var document = new BsonDocument
            {
                { "UserId", 1000 },
                { "Name", "Alice" },
                { "Email", "alice@example.com" }
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            await collection.InsertOneAsync(document);
            stopwatch.Stop();

            Console.WriteLine($"MongoDB: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }



        // 批量插入
        public static async Task InsertBatchRecords()
        {
            var collection = GetCollection();
            var documents = LoadUsersFromJson("users.json");

            Stopwatch stopwatch = Stopwatch.StartNew();
            await collection.InsertManyAsync(documents);
            stopwatch.Stop();

            Console.WriteLine($"MongoDB: Batch Insert ({documents.Count} records) - {stopwatch.ElapsedMilliseconds} ms");
        }


        // 单条查询
        public static async Task QuerySingleRecord()
        {
            var collection = GetCollection();
            var filter = Builders<BsonDocument>.Filter.Eq("UserId", 1000); // 假设查询 UserId 为 1 的用户

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            stopwatch.Stop();

            Console.WriteLine($"MongoDB: Single Query - {stopwatch.ElapsedMilliseconds} ms");

            // 输出查询结果
            if (result != null)
            {
                Console.WriteLine($"Query Result: UserId={result["UserId"]}, Name={result["Name"]}, Email={result["Email"]}");
            }
            else
            {
                Console.WriteLine("No results found for UserId=1");
            }
        }


        // 并发插入
        public static async Task ConcurrentInsert()
        {
            var collection = GetCollection();
            var documents = LoadUsersFromJson("users.json"); // 从 JSON 文件加载数据
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (var doc in documents)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await collection.InsertOneAsync(doc);
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"MongoDB: Concurrent Insert ({documents.Count} records) - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static async Task ClearCollection()
        {
            var collection = GetCollection();

            // 删除所有文档
            await collection.DeleteManyAsync(Builders<BsonDocument>.Filter.Empty);

            Console.WriteLine("MongoDB: Collection cleared successfully.");
        }

        // 并发查询
        public static async Task ConcurrentQuery()
        {
            var collection = GetCollection();
            var random = new Random();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            // 假设随机查询 UserId 的范围为 1 到 100
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var userId = random.Next(1, 101); // 随机生成 UserId
                    var filter = Builders<BsonDocument>.Filter.Eq("UserId", userId);

                    var result = await collection.Find(filter).FirstOrDefaultAsync();

                    // 可选调试输出
                    if (result != null)
                    {
                        Console.WriteLine($"Query Result: UserId={result["UserId"]}, Name={result["Name"]}, Email={result["Email"]}");
                    }
                    else
                    {
                        Console.WriteLine($"Query for UserId={userId} returned no results.");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"MongoDB: Concurrent Query Completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}

