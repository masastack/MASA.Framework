// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensitions
{
    internal static ILogger Logger { get; private set; }

    public static IServiceCollection AddMASAStackClickhouse(this IServiceCollection services, string connectionStr, string? logTable = null, string? traceTable = null)
    {
        return services.AddScoped(services => new MasaStackClickhouseConnection(connectionStr, logTable, traceTable))
            .AddScoped<ILogService, LogService>()
            .AddScoped<ITraceService, TraceService>();
    }

    public static IServiceCollection AddMASAStackClickhouse(this IServiceCollection services, string connectionStr, string logTable, string traceTable, string logSourceTable, string traceSourceTable)
    {
        services.AddScoped(services => new MasaStackClickhouseConnection(connectionStr, logTable, logSourceTable, traceTable, traceSourceTable))
            .AddScoped<ILogService, LogService>()
            .AddScoped<ITraceService, TraceService>();
        Init(services);
        return services;
    }

    private static void Init(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logfactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        Logger = logfactory.CreateLogger("Masa.Contrib.StackSdks.Tsc.Clickhouse");
        var connection = serviceProvider.GetRequiredService<MasaStackClickhouseConnection>();
        InitTable(connection);
        InitMappingTable(connection);
    }

    private static void InitTable(MasaStackClickhouseConnection connection)
    {
        string database = connection.ConnectionSettings?.Database;
        database ??= new ClickHouseConnectionSettings(connection.ConnectionString).Database;

        if (Convert.ToInt32(connection.ExecuteScalar($"select * from system.tables where database ='{database}' and name in ['{MasaStackClickhouseConnection.TraceTable}','{MasaStackClickhouseConnection.LogTable}']")) > 0)
            return;

        var sql = @$"CREATE TABLE {MasaStackClickhouseConnection.LogTable}
(

    `Timestamp` DateTime64(9) CODEC(Delta(8), ZSTD(1)),

    `TraceId` String CODEC(ZSTD(1)),

    `SpanId` String CODEC(ZSTD(1)),

    `TraceFlags` UInt32 CODEC(ZSTD(1)),

    `SeverityText` LowCardinality(String) CODEC(ZSTD(1)),

    `SeverityNumber` Int32 CODEC(ZSTD(1)),

    `ServiceName` LowCardinality(String) CODEC(ZSTD(1)),

    `Body` String CODEC(ZSTD(1)),

    `ResourceSchemaUrl` String CODEC(ZSTD(1)),

    `ResourceAttributes` Map(LowCardinality(String), String) CODEC(ZSTD(1)),

    `ScopeSchemaUrl` String CODEC(ZSTD(1)),

    `ScopeName` String CODEC(ZSTD(1)),

    `ScopeVersion` String CODEC(ZSTD(1)),

    `ScopeAttributes` Map(LowCardinality(String), String) CODEC(ZSTD(1)),

    `LogAttributes` Map(LowCardinality(String), String) CODEC(ZSTD(1)),

    INDEX idx_trace_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,

    INDEX idx_res_attr_key mapKeys(ResourceAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_res_attr_value mapValues(ResourceAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_scope_attr_key mapKeys(ScopeAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_scope_attr_value mapValues(ScopeAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_log_attr_key mapKeys(LogAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_log_attr_value mapValues(LogAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_body Body TYPE tokenbf_v1(32768, 3, 0) GRANULARITY 1
)
ENGINE = MergeTree
PARTITION BY toYYYYMM(Timestamp)
ORDER BY (
Timestamp,
ServiceName,
SeverityText,
TraceId,
SpanId
)
TTL toDateTime(Timestamp) + toIntervalDay(30)
SETTINGS index_granularity = 8192,
 ttl_only_drop_parts = 1;

CREATE TABLE {MasaStackClickhouseConnection.TraceTable}
(

    `Timestamp` DateTime64(9) CODEC(Delta(8),
 ZSTD(1)),

    `TraceId` String CODEC(ZSTD(1)),

    `SpanId` String CODEC(ZSTD(1)),

    `ParentSpanId` String CODEC(ZSTD(1)),

    `TraceState` String CODEC(ZSTD(1)),

    `SpanName` LowCardinality(String) CODEC(ZSTD(1)),

    `SpanKind` LowCardinality(String) CODEC(ZSTD(1)),

    `ServiceName` LowCardinality(String) CODEC(ZSTD(1)),

    `ResourceAttributes` Map(LowCardinality(String),
 String) CODEC(ZSTD(1)),

    `ScopeName` String CODEC(ZSTD(1)),

    `ScopeVersion` String CODEC(ZSTD(1)),

    `SpanAttributes` Map(LowCardinality(String),
 String) CODEC(ZSTD(1)),

    `Duration` Int64 CODEC(ZSTD(1)),

    `StatusCode` LowCardinality(String) CODEC(ZSTD(1)),

    `StatusMessage` String CODEC(ZSTD(1)),

    `Events.Timestamp` Array(DateTime64(9)) CODEC(ZSTD(1)),

    `Events.Name` Array(LowCardinality(String)) CODEC(ZSTD(1)),

    `Events.Attributes` Array(Map(LowCardinality(String),
 String)) CODEC(ZSTD(1)),

    `Links.TraceId` Array(String) CODEC(ZSTD(1)),

    `Links.SpanId` Array(String) CODEC(ZSTD(1)),

    `Links.TraceState` Array(String) CODEC(ZSTD(1)),

    `Links.Attributes` Array(Map(LowCardinality(String),
 String)) CODEC(ZSTD(1)),

    INDEX idx_trace_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,

    INDEX idx_res_attr_key mapKeys(ResourceAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_res_attr_value mapValues(ResourceAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_span_attr_key mapKeys(SpanAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_span_attr_value mapValues(SpanAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,

    INDEX idx_duration Duration TYPE minmax GRANULARITY 1
)
ENGINE = MergeTree
PARTITION BY toYYYYMM(Timestamp)
ORDER BY (
Timestamp,
ServiceName,
 TraceId
 )
TTL toDateTime(Timestamp) + toIntervalDay(30)
SETTINGS index_granularity = 8192,
 ttl_only_drop_parts = 1;

CREATE MATERIALIZED VIEW {MasaStackClickhouseConnection.LogTable}_v TO {MasaStackClickhouseConnection.LogTable}
AS
SELECT * FROM {MasaStackClickhouseConnection.LogSourceTable};

CREATE MATERIALIZED VIEW {MasaStackClickhouseConnection.TraceTable}_v TO {MasaStackClickhouseConnection.TraceTable}
AS
SELECT * FROM {MasaStackClickhouseConnection.TraceSourceTable};
";

        connection.ExecuteScalar(sql);
    }

    private static void InitMappingTable(MasaStackClickhouseConnection connection)
    {
        string database = connection.ConnectionSettings?.Database;
        database ??= new ClickHouseConnectionSettings(connection.ConnectionString).Database;

        if (Convert.ToInt32(connection.ExecuteScalar($"select * from system.tables where database ='{database}' and name in ['{MasaStackClickhouseConnection.MappingTable.Split('.').Last()}']")) > 0)
            return;

        var sql = @$"
CREATE MATERIALIZED VIEW {database}.v_otel_traces_attribute_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(SpanAttributes)) as Name, 'trace_attributes' as Type
from {MasaStackClickhouseConnection.TraceTable};

CREATE MATERIALIZED VIEW {database}.v_otel_traces_resource_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(ResourceAttributes)) as Name, 'trace_resource' as Type
from {MasaStackClickhouseConnection.TraceTable};

CREATE MATERIALIZED VIEW {database}.v_otel_logs_attribute_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(LogAttributes)) as Name, 'log_attributes' as Type
from {MasaStackClickhouseConnection.LogTable};

CREATE MATERIALIZED VIEW {database}.v_otel_logs_resource_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(ResourceAttributes)) as Name, 'log_resource' as Type
from {MasaStackClickhouseConnection.LogTable};

insert into {MasaStackClickhouseConnection.MappingTable}
values (['Timestamp','TraceId','SpanId','TraceFlag','SeverityText','SeverityNumber','Body'],'log_basic'),
(['Timestamp','TraceId','SpanId','ParentSpanId','TraceState','SpanKind','Duration'],'trace_basic');
";

        connection.ExecuteScalar(sql);
    }
}
