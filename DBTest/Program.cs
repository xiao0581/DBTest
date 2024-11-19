
using DBTest;

//// SQL Server
//SqlServerTest.InsertSingleRecord();
//SqlServerTest.InsertBatchRecords();
//SqlServerTest.QuerySingleRecord();
//SqlServerTest.QueryComplexJoin();
//SqlServerTest.ConcurrentReadWrite();

//// MongoDB
//await MongoDBTest.InsertSingleRecord();
//await MongoDBTest.InsertBatchRecords();
//await MongoDBTest.QuerySingleRecord();
//await MongoDBTest.QueryComplexJoin();
//await MongoDBTest.ConcurrentReadWrite();

//// Redis
//RedisTest.InsertSingleRecord();
//RedisTest.InsertBatchRecords();
//RedisTest.QuerySingleRecord();
//RedisTest.QueryComplexJoin();
//RedisTest.ConcurrentReadWrite();

//// Cassandra
//CassandraTest.InsertSingleRecord();
//CassandraTest.InsertBatchRecords();
//CassandraTest.QuerySingleRecord();
//CassandraTest.QueryComplexJoin();
//CassandraTest.ConcurrentReadWrite();

//// Neo4j
//await Neo4jTest.InsertSingleRecord();
//await Neo4jTest.InsertBatchRecords();
//await Neo4jTest.QuerySingleRecord();
//await Neo4jTest.QueryComplexJoin();
//await Neo4jTest.ConcurrentReadWrite();

DBsetup db = new DBsetup();
db.trySql();