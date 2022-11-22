// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Model;

internal class ElasticseachTraceDatabaseResponseDto: TraceDatabaseResponseDto
{
    [JsonPropertyName("db.system")]
    public override string System { get; set; }

    [JsonPropertyName("db.connection_string")]
    public override string ConnectionString { get; set; }

    [JsonPropertyName("db.user")]
    public override string User { get; set; }

    [JsonPropertyName("net.peer.ip")]
    public override string PeerIp { get; set; }

    [JsonPropertyName("net.peer.name")]
    public override string PeerName { get; set; }

    [JsonPropertyName("net.peer.port")]
    public override int PeerPort { get; set; }

    [JsonPropertyName("net.transport")]
    public override string Transport { get; set; }

    [JsonPropertyName("db.jdbc.driver_classname")]
    public override string JdbcDriverClassName { get; set; }

    [JsonPropertyName("db.mssql.instance_name")]
    public override string MssqlInstanceName { get; set; }

    [JsonPropertyName("db.name")]
    public override string Name { get; set; }

    [JsonPropertyName("db.statement")]
    public override string Statement { get; set; }

    [JsonPropertyName("db.operation")]
    public override string Operation { get; set; }

    [JsonPropertyName("db.redis.database_index")]
    public override int RedisDatabaseIndex { get; set; }

    [JsonPropertyName("db.mongodb.collection")]
    public override string MongodbCollection { get; set; }

    [JsonPropertyName("db.sql.table")]
    public override string SqlTable { get; set; }

    #region Cassandra

    [JsonPropertyName("db.cassandra.page_size")]
    public override int CassandraPageSize { get; set; }

    [JsonPropertyName("db.cassandra.consistency_level")]
    public override string CassandraConsistencyLevel { get; set; }

    [JsonPropertyName("db.cassandra.table")]
    public override string CassandraTable { get; set; }

    [JsonPropertyName("db.cassandra.idempotence")]
    public override bool CassandraIdempotence { get; set; }

    [JsonPropertyName("db.cassandra.speculative_execution_count")]
    public override bool CassandraSpeculativeExecutionCount { get; set; }

    [JsonPropertyName("db.cassandra.coordinator.id")]
    public override string CassandraCoordinatorId { get; set; }

    [JsonPropertyName("db.cassandra.coordinator.dc")]
    public override string CassandraCoordinatorDc { get; set; }

    #endregion
}
