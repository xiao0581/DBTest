using Cassandra;
using Neo4j.Driver;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTest
{
    public class Neo4jTest
    {
        private static IDriver GetDriver()
        {
            return GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password"));
        }

        public static async Task InsertSingleRecord()
        {
            var driver = GetDriver();
            var session = driver.AsyncSession();

            Stopwatch stopwatch = Stopwatch.StartNew();
            await session.RunAsync("CREATE (u:User {UserId: 1, Name: 'Alice', Email: 'alice@example.com'})");
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Single Insert - {stopwatch.ElapsedMilliseconds} ms");

            await session.CloseAsync();
            await driver.CloseAsync();
        }

        public static async Task InsertBatchRecords()
        {
            var driver = GetDriver();
            var session = driver.AsyncSession();

            Stopwatch stopwatch = Stopwatch.StartNew();
            await session.RunAsync("CREATE (u1:User {UserId: 2, Name: 'Bob', Email: 'bob@example.com'}), " +
                                   "(u2:User {UserId: 3, Name: 'Charlie', Email: 'charlie@example.com'})");
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Batch Insert - {stopwatch.ElapsedMilliseconds} ms");

            await session.CloseAsync();
            await driver.CloseAsync();
        }

        public static async Task QuerySingleRecord()
        {
            var driver = GetDriver();
            var session = driver.AsyncSession();

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = await session.RunAsync("MATCH (u:User {UserId: 1}) RETURN u.Name AS Name, u.Email AS Email");
            var record = await result.SingleAsync();
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Single Query - {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Neo4j: Query Result - {record["Name"]}, {record["Email"]}");

            await session.CloseAsync();
            await driver.CloseAsync();
        }

        public static async Task QueryComplexJoin()
        {
            Console.WriteLine("Neo4j: Complex Query (Join) supported but requires relationship setup");
        }

        public static async Task ConcurrentReadWrite()
        {
            var driver = GetDriver();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var userId = i + 10;
                tasks.Add(Task.Run(() => driver.AsyncSession().RunAsync($"CREATE (u:User {{UserId: {userId}, Name: 'User{userId}', Email: 'user{userId}@example.com'}})")));
                tasks.Add(Task.Run(() => driver.AsyncSession().RunAsync($"MATCH (u:User {{UserId: {userId}}}) RETURN u")));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Neo4j: Concurrent Read/Write Test Completed in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
