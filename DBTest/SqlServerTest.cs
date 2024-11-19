using Cassandra.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTest
{
    public class SqlServerTest
    {
        private static string connectionString = "Data Source=XIAO-PC\\XIAODATA;Integrated Security=True;Database=DBtest;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static void InsertSingleRecord()
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

        public static void InsertBatchRecords()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var query = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
            var parameters = new[]
            {
            new { UserId = 2, Name = "Bob", Email = "bob@example.com" },
            new { UserId = 3, Name = "Charlie", Email = "charlie@example.com" }
        };

            Stopwatch stopwatch = Stopwatch.StartNew();
            connection.Execute(query, parameters);
            stopwatch.Stop();
            Console.WriteLine($"SQL Server: Batch Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void QuerySingleRecord()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var query = "SELECT * FROM Users WHERE UserId = @UserId";
            var parameters = new { UserId = 1 };

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = connection.QuerySingleOrDefault(query, parameters);
            stopwatch.Stop();
            Console.WriteLine($"SQL Server: Single Query - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void QueryComplexJoin()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var query = @"
            SELECT u.Name, o.OrderId 
            FROM Users u
            JOIN Orders o ON u.UserId = o.UserId
            WHERE u.UserId = @UserId";
            var parameters = new { UserId = 1 };

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = connection.Query(query, parameters);
            stopwatch.Stop();
            Console.WriteLine($"SQL Server: Complex Query (Join) - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void ConcurrentReadWrite()
        {
            var tasks = new List<Task>();
            var random = new Random();

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    using var connection = new SqlConnection(connectionString);
                    connection.Open();

                    // 写入数据
                    var userId = random.Next(1000, 2000);
                    var insertQuery = "INSERT INTO Users (UserId, Name, Email) VALUES (@UserId, @Name, @Email)";
                    var insertParameters = new { UserId = userId, Name = $"User{userId}", Email = $"user{userId}@example.com" };
                    connection.Execute(insertQuery, insertParameters);

                    // 读取数据
                    var readQuery = "SELECT * FROM Users WHERE UserId = @UserId";
                    var readParameters = new { UserId = userId };
                    connection.QuerySingleOrDefault(readQuery, readParameters);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();
            Console.WriteLine($"SQL Server: Concurrent Read/Write Test Completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
