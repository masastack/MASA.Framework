// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public class TraceDatabaseResponseDto
{
    public virtual string Kind { get; set; }
    public virtual string System { get; set; }
    public virtual string ConnectionString { get; set; }
    public virtual string User { get; set; }
    public virtual string PeerIp { get; set; }
    public virtual string PeerName { get; set; }
    public virtual int PeerPort { get; set; }
    public virtual string Transport { get; set; }
    public virtual string JdbcDriverClassName { get; set; }
    public virtual string MssqlInstanceName { get; set; }
    public virtual string Name { get; set; }
    public virtual string Statement { get; set; }
    public virtual string Operation { get; set; }
    public virtual int RedisDatabaseIndex { get; set; }
    public virtual string MongodbCollection { get; set; }
    public virtual string SqlTable { get; set; }

    #region Cassandra

    public virtual int CassandraPageSize { get; set; }
    public virtual string CassandraConsistencyLevel { get; set; }
    public virtual string CassandraTable { get; set; }
    public virtual bool CassandraIdempotence { get; set; }
    public virtual bool CassandraSpeculativeExecutionCount { get; set; }
    public virtual string CassandraCoordinatorId { get; set; }
    public virtual string CassandraCoordinatorDc { get; set; }
    #endregion
}
