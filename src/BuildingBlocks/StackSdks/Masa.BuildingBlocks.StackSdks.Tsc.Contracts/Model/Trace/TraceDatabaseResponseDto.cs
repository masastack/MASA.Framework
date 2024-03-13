// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceDatabaseResponseDto
{
    [JsonPropertyName("db.system")]
    public virtual string System { get; set; }

    [JsonPropertyName("db.connection_string")]
    public virtual string ConnectionString { get; set; }

    [JsonPropertyName("db.user")]
    public virtual string User { get; set; }

    [JsonPropertyName("net.peer.ip")]
    public virtual string PeerIp { get; set; }

    [JsonPropertyName("net.peer.name")]
    public virtual string PeerName { get; set; }

    [JsonPropertyName("net.peer.port")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int PeerPort { get; set; }

    [JsonPropertyName("net.transport")]
    public virtual string Transport { get; set; }

    [JsonPropertyName("db.jdbc.driver_classname")]
    public virtual string JdbcDriverClassName { get; set; }

    [JsonPropertyName("db.mssql.instance_name")]
    public virtual string MssqlInstanceName { get; set; }

    [JsonPropertyName("db.name")]
    public virtual string Name { get; set; }

    [JsonPropertyName("db.statement")]
    public virtual string Statement { get; set; }

    [JsonPropertyName("db.operation")]
    public virtual string Operation { get; set; }

    [JsonPropertyName("db.redis.database_index")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int RedisDatabaseIndex { get; set; }

    [JsonPropertyName("db.mongodb.collection")]
    public virtual string MongodbCollection { get; set; }

    [JsonPropertyName("db.sql.table")]
    public virtual string SqlTable { get; set; }

    #region Cassandra

    [JsonPropertyName("db.cassandra.page_size")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public virtual int CassandraPageSize { get; set; }

    [JsonPropertyName("db.cassandra.consistency_level")]
    public virtual string CassandraConsistencyLevel { get; set; }

    [JsonPropertyName("db.cassandra.table")]
    public virtual string CassandraTable { get; set; }

    [JsonPropertyName("db.cassandra.idempotence")]
    public virtual bool CassandraIdempotence { get; set; }

    [JsonPropertyName("db.cassandra.speculative_execution_count")]
    public virtual bool CassandraSpeculativeExecutionCount { get; set; }

    [JsonPropertyName("db.cassandra.coordinator.id")]
    public virtual string CassandraCoordinatorId { get; set; }

    [JsonPropertyName("db.cassandra.coordinator.dc")]
    public virtual string CassandraCoordinatorDc { get; set; }

    #endregion
}
