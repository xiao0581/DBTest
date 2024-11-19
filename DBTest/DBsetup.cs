using Dapper;
using Microsoft.Data.SqlClient;
using MongoDB.Driver.Core.Configuration;
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
        public void tryMongo()
        {
            MongoDBTest.InsertSingleRecord();
            MongoDBTest.InsertBatchRecords();
            MongoDBTest.QuerySingleRecord();
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
        public void tryCassandra()
        {
            CassandraTest.InsertSingleRecord();
            CassandraTest.InsertBatchRecords();
            CassandraTest.QuerySingleRecord();
        }
        public void tryNeo4j()
        {
            Neo4jTest.InsertSingleRecord();
            Neo4jTest.InsertBatchRecords();
            Neo4jTest.QuerySingleRecord();
        }
    }
}
