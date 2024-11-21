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
               
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("TestDB");
                var collection = database.GetCollection<BsonDocument>("Users");

            
                var document = new BsonDocument
        {
            { "UserId", 1 },
            { "Name", "Alice" },
            { "Email", "alice@example.com" }
        };

               
                Stopwatch stopwatch = Stopwatch.StartNew();
                collection.InsertOne(document); 

                Console.WriteLine($"MongoDB: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void tryRedis()
        {
           
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

         
            IDatabase db = redis.GetDatabase();

       
            db.StringSet("name", "Alice");

   
            string name = db.StringGet("name");

    
            Console.WriteLine($"Name: {name}");

        
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
                using var session = driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write)); 

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
                session.RunAsync(cypherQuery, parameters).Wait(); 
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
