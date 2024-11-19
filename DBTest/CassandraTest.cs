using Cassandra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTest
{
    public class CassandraTest
    {
        private static ISession GetSession()
        {
            var cluster = Cluster.Builder().AddContactPoint("localhost").Build();
            var session = cluster.Connect("TestDB");
            session.Execute("CREATE TABLE IF NOT EXISTS Users (UserId int PRIMARY KEY, Name text, Email text);");
            return session;
        }

        public static void InsertSingleRecord()
        {
            var session = GetSession();
            Stopwatch stopwatch = Stopwatch.StartNew();
            session.Execute("INSERT INTO Users (UserId, Name, Email) VALUES (1, 'Alice', 'alice@example.com');");
            stopwatch.Stop();
            Console.WriteLine($"Cassandra: Single Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void InsertBatchRecords()
        {
            var session = GetSession();
            Stopwatch stopwatch = Stopwatch.StartNew();
            session.Execute("BEGIN BATCH " +
                            "INSERT INTO Users (UserId, Name, Email) VALUES (2, 'Bob', 'bob@example.com');" +
                            "INSERT INTO Users (UserId, Name, Email) VALUES (3, 'Charlie', 'charlie@example.com');" +
                            "APPLY BATCH;");
            stopwatch.Stop();
            Console.WriteLine($"Cassandra: Batch Insert - {stopwatch.ElapsedMilliseconds} ms");
        }

        public static void QuerySingleRecord()
        {
            var session = GetSession();
            Stopwatch stopwatch = Stopwatch.StartNew();
            var row = session.Execute("SELECT * FROM Users WHERE UserId = 1").FirstOrDefault();
            stopwatch.Stop();
            Console.WriteLine($"Cassandra: Single Query - {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Cassandra: Query Result - {row["Name"]}, {row["Email"]}");
        }

        public static void QueryComplexJoin()
        {
            Console.WriteLine("Cassandra: Complex Query (Join) not supported");
        }

        public static void ConcurrentReadWrite()
        {
            var session = GetSession();
            var tasks = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var userId = i + 10;
                tasks.Add(Task.Run(() => session.Execute($"INSERT INTO Users (UserId, Name, Email) VALUES ({userId}, 'User{userId}', 'user{userId}@example.com');")));
                tasks.Add(Task.Run(() => session.Execute($"SELECT * FROM Users WHERE UserId = {userId}")));
            }

            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();
            Console.WriteLine($"Cassandra: Concurrent Read/Write Test Completed in {stopwatch.ElapsedMilliseconds} ms");
        }

    }
}
