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
        //private static IDriver GetDriver()
        //{
        //    return GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "12345678"));
        //}

      
        //private static List<User> LoadUsersFromJson(string filePath)
        //{
        //    var jsonData = File.ReadAllText(filePath);
        //    return JsonConvert.DeserializeObject<List<User>>(jsonData);
        //}

       
        //public static async Task InsertSingleRecord()
        //{
        //    var driver = GetDriver();
        //    var session = driver.AsyncSession();

        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    await session.RunAsync("CREATE (u:User {UserId: 1000, Name: 'Alice', Email: 'alice@example.com'})");
        //    stopwatch.Stop();
        //    Console.WriteLine($"Neo4j: Single Insert - {stopwatch.ElapsedMilliseconds} ms");

        //    await session.CloseAsync();
        //    await driver.CloseAsync();
        //}

     
        //public static async Task InsertBatchRecords()
        //{
        //    var driver = GetDriver();
        //    var session = driver.AsyncSession();
        //    var users = LoadUsersFromJson("users.json"); 

        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    foreach (var user in users)
        //    {
        //        await session.RunAsync($"CREATE (u:User {{UserId: {user.UserId}, Name: '{user.Name}', Email: '{user.Email}'}})");
        //    }
        //    stopwatch.Stop();
        //    Console.WriteLine($"Neo4j: Batch Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");

        //    await session.CloseAsync();
        //    await driver.CloseAsync();
        //}

       
        //public static async Task QuerySingleRecord()
        //{
        //    var driver = GetDriver();
        //    var session = driver.AsyncSession();

        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    var result = await session.RunAsync("MATCH (u:User {UserId: 1000}) RETURN u.Name AS Name, u.Email AS Email");
        //    var record = await result.SingleAsync();
        //    stopwatch.Stop();

        //    Console.WriteLine($"Neo4j: Single Query - {stopwatch.ElapsedMilliseconds} ms");
            

        //    await session.CloseAsync();
        //    await driver.CloseAsync();
        //}
        //public static async Task ClearUsersAsync()
        //{
        //    using var driver = GetDriver();
        //    using var session = driver.AsyncSession();

        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    try
        //    {
        //        await session.RunAsync("MATCH (u:User) DETACH DELETE u");
        //        stopwatch.Stop();
        //        Console.WriteLine($"Neo4j: Cleared all User nodes in {stopwatch.ElapsedMilliseconds} ms");
        //    }
        //    finally
        //    {
        //        await session.CloseAsync();
        //        await driver.CloseAsync();
        //    }
        //}
        
        //public static async Task ConcurrentInsert()
        //{
        //    var driver = GetDriver();
        //    var users = LoadUsersFromJson("users.json"); 
        //    var tasks = new List<Task>();

        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    foreach (var user in users)
        //    {
        //        tasks.Add(Task.Run(async () =>
        //        {
        //            var session = driver.AsyncSession();
        //            await session.RunAsync($"CREATE (u:User {{UserId: {user.UserId}, Name: '{user.Name}', Email: '{user.Email}'}})");
        //            await session.CloseAsync();
        //        }));
        //    }

        //    await Task.WhenAll(tasks);
        //    stopwatch.Stop();
        //    Console.WriteLine($"Neo4j: Concurrent Insert ({users.Count} records) - {stopwatch.ElapsedMilliseconds} ms");

        //    await driver.CloseAsync();
        //}

  
        //public static async Task ConcurrentQuery()
        //{
        //    var driver = GetDriver();
        //    var random = new Random();
        //    var tasks = new List<Task>();

        //    Stopwatch stopwatch = Stopwatch.StartNew();

           
        //    for (int i = 0; i < 100; i++)
        //    {
        //        tasks.Add(Task.Run(async () =>
        //        {
        //            var session = driver.AsyncSession();
        //            try
        //            {
        //                var userId = random.Next(1, 101); 
        //                var result = await session.RunAsync($"MATCH (u:User {{UserId: {userId}}}) RETURN u.Name AS Name, u.Email AS Email");
                                
        //            }
        //            finally
        //            {
        //                await session.CloseAsync();
        //            }
        //        }));
        //    }

        //    await Task.WhenAll(tasks);
        //    stopwatch.Stop();
        //    Console.WriteLine($"Neo4j: Concurrent Query Completed in {stopwatch.ElapsedMilliseconds} ms");

        //    await driver.CloseAsync();
        //}

    }
}
