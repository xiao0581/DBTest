using Cassandra;
using Neo4j.Driver;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DBTest
{
    public class Neo4jTest
    {
        private static IDriver GetDriver()
        {
            return GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));
        }

        // 加载 JSON 数据
        private static List<User> LoadUsersFromJson(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<User>>(jsonData);
        }

        // 单条记录插入
        public static async Task InsertSingleRecord()
        {
            var driver = GetDriver();
            var session = driver.AsyncSession();

            Stopwatch stopwatch = Stopwatch.StartNew();
            await session.RunAsync("CREATE (u:User {UserId: 1000, Name: 'Alice', Email: 'alice@example.com'})");
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Single Insert - {stopwatch.ElapsedMilliseconds} ms");

            await session.CloseAsync();
            await driver.CloseAsync();
        }

        // 批量插入
        public static async Task InsertBatchRecords()
        {
            var driver = GetDriver();
            var session = driver.AsyncSession();
            var users = LoadUsersFromJson("users.json"); // 从 JSON 文件加载用户数据

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var user in users)
            {
                await session.RunAsync($"CREATE (u:User {{UserId: {user.UserId}, Name: '{user.Name}', Email: '{user.Email}'}})");
            }
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Batch Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");

            await session.CloseAsync();
            await driver.CloseAsync();
        }

        // 单条查询
        public static async Task QuerySingleRecord()
        {
            var driver = GetDriver();
            var session = driver.AsyncSession();

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = await session.RunAsync("MATCH (u:User {UserId: 1000}) RETURN u.Name AS Name, u.Email AS Email");
            var record = await result.SingleAsync();
            stopwatch.Stop();

            Console.WriteLine($"Neo4j: Single Query - {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Neo4j: Query Result - {record["Name"]}, {record["Email"]}");

            await session.CloseAsync();
            await driver.CloseAsync();
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
        // 并发插入
        public static async Task ConcurrentInsert()
        {
            var driver = GetDriver();
            var users = LoadUsersFromJson("users.json"); // 从 JSON 文件加载用户数据
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (var user in users)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var session = driver.AsyncSession();
                    await session.RunAsync($"CREATE (u:User {{UserId: {user.UserId}, Name: '{user.Name}', Email: '{user.Email}'}})");
                    await session.CloseAsync();
                }));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Concurrent Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");

            await driver.CloseAsync();
        }

        // 并发查询
        public static async Task ConcurrentQuery()
        {
            var driver = GetDriver();
            var random = new Random();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            // 假设随机查询 UserId 的范围为 1 到 100
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var session = driver.AsyncSession();
                    try
                    {
                        var userId = random.Next(1, 101); // 随机生成 UserId
                        var result = await session.RunAsync($"MATCH (u:User {{UserId: {userId}}}) RETURN u.Name AS Name, u.Email AS Email");

                        // 将结果转换为列表
                        var records = await result.ToListAsync();

                        // 使用 SingleOrDefault 获取单条记录
                        var record = records.SingleOrDefault();
                        if (record != null)
                        {
                            Console.WriteLine($"Query Result: UserId={userId}, Name={record["Name"]}, Email={record["Email"]}");
                        }
                        else
                        {
                            Console.WriteLine($"Query for UserId={userId} returned no results.");
                        }
                    }
                    finally
                    {
                        await session.CloseAsync();
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
