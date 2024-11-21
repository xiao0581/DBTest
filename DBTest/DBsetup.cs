using Dapper;
using Microsoft.Data.SqlClient;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Neo4j.Driver;
using Npgsql;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTest
{
    public class DBsetup
    {
        private static string connectionString = "Data Source=XIAO-PC\\XIAODATA;Integrated Security=True;Database=DBtest;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private static string postgreConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=12345;Database=Test";
        public void trySql()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var query = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
            var parameters = new { UserId = 1, Name = "Alice", Email = "alice@example.com" };

            Stopwatch stopwatch = Stopwatch.StartNew();
            connection.Execute(query, parameters);
            stopwatch.Stop();
            Console.WriteLine($"SQL Server: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }
        public void TryMongo()
        {
            try
            {
                // 连接到 MongoDB
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("TestDB");
                var collection = database.GetCollection<BsonDocument>("Users");

                // 准备插入的数据
                var document = new BsonDocument
        {
            { "UserId", 1 },
            { "Name", "Alice" },
            { "Email", "alice@example.com" }
        };

                // 插入数据并计时
                Stopwatch stopwatch = Stopwatch.StartNew();
                collection.InsertOne(document); // 使用同步插入方法
                stopwatch.Stop();

                Console.WriteLine($"MongoDB: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void tryRedis()
        {
            // 连接Redis服务器
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

            // 获取默认的数据库实例
            IDatabase db = redis.GetDatabase();

            // 插入数据
            db.StringSet("name", "Alice");

            // 读取数据
            string name = db.StringGet("name");

            // 输出数据
            Console.WriteLine($"Name: {name}");

            // 关闭连接
            redis.Close();
        }
        public void tryPostgre()
        {
            using var connection = new NpgsqlConnection(postgreConnectionString);
            connection.Open();

            var query = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
            var parameters = new { UserId = 1, Name = "Alice", Email = "alice@example.com" };

            Stopwatch stopwatch = Stopwatch.StartNew();
            connection.Execute(query, parameters);
            stopwatch.Stop();

            Console.WriteLine($"PostgreSQL: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }
        public void tryNeo4j()
        {
            string uri = "bolt://localhost:7687";
            string username = "neo4j";
            string password = "12345678";

            try
            {
                using var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
                using var session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write)); // 使用新方法创建会话

                var cypherQuery = @"
        CREATE (u:User {UserId: $UserId, Name: $Name, Email: $Email})
    ";
                var parameters = new
                {
                    UserId = 1,
                    Name = "Alice",
                    Email = "alice@example.com"
                };

                Stopwatch stopwatch = Stopwatch.StartNew();
                session.RunAsync(cypherQuery, parameters).Wait(); // 使用异步执行并等待完成
                stopwatch.Stop();

                Console.WriteLine($"Neo4j: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
    }
}
