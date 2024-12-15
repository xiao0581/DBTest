
using DBTest;
//await SqlServerTest.ClearDatabaseAsync();
//await MongoDBTest.ClearCollection();
//await RedisTest.ClearAllDataAsync();
//await PostgreSQLTest.ClearUsersTableAsync();
////await Neo4jTest.ClearUsersAsync();
//Console.WriteLine("Starting Database Tests...");

////batch insert
//await SqlServerTest.InsertBatchRecordsAsync();
//await PostgreSQLTest.InsertBatchRecordsAsync();
//await MongoDBTest.InsertBatchRecords();
//await RedisTest.InsertBatchRecordsAsync();


//// SQL Server Tests
//Console.WriteLine("\n=== SQL Server Tests ===");
//await SqlServerTest.InsertSingleRecordAsync();
//await SqlServerTest.InsertBatchRecordsAsync();
//await SqlServerTest.QuerySingleRecordAsync();
//await SqlServerTest.ClearDatabaseAsync();
//await SqlServerTest.ConcurrentInsertAsync();
//await SqlServerTest.ConcurrentQueryAsync();


//// PostgreSQL Tests
//Console.WriteLine("\n=== PostgreSQL Tests ===");
//await PostgreSQLTest.InsertSingleRecordAsync();
//await PostgreSQLTest.InsertBatchRecordsAsync();
//await PostgreSQLTest.QuerySingleRecordAsync();
//await PostgreSQLTest.ClearUsersTableAsync();
//await PostgreSQLTest.ConcurrentInsertAsync();
//await PostgreSQLTest.ConcurrentQueryAsync();

//// MongoDB Tests
//Console.WriteLine("\n=== MongoDB Tests ===");
//await MongoDBTest.InsertSingleRecord();
//await MongoDBTest.InsertBatchRecords();
//await MongoDBTest.QuerySingleRecord();
//await MongoDBTest.ClearCollection();
//await MongoDBTest.ConcurrentInsert();
//await MongoDBTest.ConcurrentQuery();


//// Redis Tests
//Console.WriteLine("\n=== Redis Tests ===");
//await RedisTest.InsertSingleRecordAsync();
//await RedisTest.InsertBatchRecordsAsync();
//await RedisTest.QuerySingleRecordAsync();
//await RedisTest.ClearAllDataAsync();
//await RedisTest.ConcurrentInsertAsync();
//await RedisTest.ConcurrentQueryAsync();

////// Neo4j Tests
////Console.WriteLine("\n=== Neo4j Tests ===");
////await Neo4jTest.InsertSingleRecord();
////await Neo4jTest.InsertBatchRecords();
////await Neo4jTest.QuerySingleRecord();
////await Neo4jTest.ClearUsersAsync();
////await Neo4jTest.ConcurrentInsert();
////await Neo4jTest.ConcurrentQuery();


//SQL server vs MongoDB complex query Test
await SqlServerTest.ComplexQueryRecordAsync();
await MongoDBTest.ComplexQueryMongoAsync();
Console.WriteLine("\nAll tests completed.");