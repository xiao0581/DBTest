﻿
using DBTest;

Console.WriteLine("Starting Database Tests...");

// SQL Server Tests
Console.WriteLine("\n=== SQL Server Tests ===");
await SqlServerTest.InsertSingleRecordAsync();
await SqlServerTest.InsertBatchRecordsAsync();
await SqlServerTest.QuerySingleRecordAsync();
await SqlServerTest.ClearDatabaseAsync();
await SqlServerTest.ConcurrentInsertAsync();
await SqlServerTest.ConcurrentQueryAsync();

// MongoDB Tests
Console.WriteLine("\n=== MongoDB Tests ===");
await MongoDBTest.InsertSingleRecord();
await MongoDBTest.InsertBatchRecords();
await MongoDBTest.QuerySingleRecord();
await MongoDBTest.ClearCollection();
await MongoDBTest.ConcurrentInsert();
await MongoDBTest.ConcurrentQuery();

// Redis Tests
Console.WriteLine("\n=== Redis Tests ===");
await RedisTest.InsertSingleRecord();
await RedisTest.InsertBatchRecords();
await RedisTest.QuerySingleRecord();
await RedisTest.ClearUsersAsync();
await RedisTest.ConcurrentInsert();
await RedisTest.ConcurrentQuery();

// PostgreSQL Tests
Console.WriteLine("\n=== PostgreSQL Tests ===");
await PostgreSQLTest.InsertSingleRecordAsync();
await PostgreSQLTest.InsertBatchRecordsAsync();
await PostgreSQLTest.QuerySingleRecordAsync();
await PostgreSQLTest.ClearUsersTableAsync();
await PostgreSQLTest.ConcurrentInsertAsync();
await PostgreSQLTest.ConcurrentQueryAsync();

// Neo4j Tests
Console.WriteLine("\n=== Neo4j Tests ===");
await Neo4jTest.InsertSingleRecord();
await Neo4jTest.InsertBatchRecords();
await Neo4jTest.QuerySingleRecord();
await Neo4jTest.ClearUsersAsync();
await Neo4jTest.ConcurrentInsert();
await Neo4jTest.ConcurrentQuery();

Console.WriteLine("\nAll tests completed.");