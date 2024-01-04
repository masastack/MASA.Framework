// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MasaTscCliclhouseExtensitions
{
    internal static ILogger? Logger { get; private set; }

    public static IServiceCollection AddMASAStackClickhouse(this IServiceCollection services, string connectionStr, string logTable, string traceTable, string? logSourceTable = null, string? traceSourceTable = null, Action<IDbConnection>? configer = null)
    {
        services.AddScoped(services => new MasaStackClickhouseConnection(connectionStr, logTable, traceTable, logSourceTable, traceSourceTable))
            .AddScoped<ILogService, LogService>()
            .AddScoped<ITraceService, TraceService>();
        Init(services);
        configer?.Invoke(services.BuildServiceProvider().GetRequiredService<MasaStackClickhouseConnection>()!);
        return services;
    }

    private static void Init(IServiceCollection services, bool createTable = true)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logfactory = serviceProvider.GetService<ILoggerFactory>();
        Logger = logfactory?.CreateLogger("Masa.Contrib.StackSdks.Tsc.Clickhouse");
        var connection = serviceProvider.GetRequiredService<MasaStackClickhouseConnection>();
        if (createTable)
            InitTable(connection);
        InitMappingTable(connection);

        var timezoneStr = GetTimezone(connection);
        MasaStackClickhouseConnection.TimeZone = TZConvert.GetTimeZoneInfo(timezoneStr);
    }

    private static void InitTable(MasaStackClickhouseConnection connection)
    {
        var database = connection.ConnectionSettings?.Database;
        database ??= new ClickHouseConnectionSettings(connection.ConnectionString).Database;

        if (Convert.ToInt32(connection.ExecuteScalar($"select count() from system.tables where database ='{database}' and name in ['{MasaStackClickhouseConnection.TraceTable.Split('.')[1]}','{MasaStackClickhouseConnection.LogTable.Split('.')[1]}']")) > 0)
            return;

        var createTableSqls = new string[]{

            @$"CREATE TABLE {MasaStackClickhouseConnection.LogTable}
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
    `Resources` String CODEC(ZSTD(1)),
    `ScopeSchemaUrl` String CODEC(ZSTD(1)),
    `ScopeName` String CODEC(ZSTD(1)),
    `ScopeVersion` String CODEC(ZSTD(1)),
    `Scopes` String CODEC(ZSTD(1)),
    `Logs` String CODEC(ZSTD(1)),
	
	`Resource.service.namespace` String CODEC(ZSTD(1)),	
	`Resource.service.version` String CODEC(ZSTD(1)),	
	`Resource.service.instance.id` String CODEC(ZSTD(1)),	
	
	`Attributes.TaskId`  String CODEC(ZSTD(1)),	
	`Attributes.exception.message`  String CODEC(ZSTD(1)),	
    
    ResourceAttributesKeys Array(String) CODEC(ZSTD(1)),
    ResourceAttributesValues Array(String) CODEC(ZSTD(1)),
    LogAttributesKeys Array(String) CODEC(ZSTD(1)),
    LogAttributesValues Array(String) CODEC(ZSTD(1)),    

    INDEX idx_log_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_serviceinstanceid `Resource.service.instance.id` TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_severitytext SeverityText TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_log_taskid `Attributes.TaskId` TYPE bloom_filter(0.001) GRANULARITY 1,
	
	INDEX idx_string_body Body TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1,
	INDEX idx_string_exceptionmessage Attributes.exception.message TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1
)
ENGINE = MergeTree
PARTITION BY toDate(Timestamp)
ORDER BY (
 Timestamp,
 `Resource.service.namespace`,
 ServiceName
 )
TTL toDateTime(Timestamp) + toIntervalDay(30)
SETTINGS index_granularity = 8192,
 ttl_only_drop_parts = 1;
",
@$"CREATE TABLE {MasaStackClickhouseConnection.TraceTable}
(
    `Timestamp` DateTime64(9) CODEC(Delta(8), ZSTD(1)),
    `TraceId` String CODEC(ZSTD(1)),
    `SpanId` String CODEC(ZSTD(1)),
    `ParentSpanId` String CODEC(ZSTD(1)),
    `TraceState` String CODEC(ZSTD(1)),
    `SpanName` LowCardinality(String) CODEC(ZSTD(1)),
    `SpanKind` LowCardinality(String) CODEC(ZSTD(1)),
    `ServiceName` LowCardinality(String) CODEC(ZSTD(1)),
    `Resources` String CODEC(ZSTD(1)),
    `ScopeName` String CODEC(ZSTD(1)),
    `ScopeVersion` String CODEC(ZSTD(1)),
    `Spans` String CODEC(ZSTD(1)),
    `Duration` Int64 CODEC(ZSTD(1)),
    `StatusCode` LowCardinality(String) CODEC(ZSTD(1)),
    `StatusMessage` String CODEC(ZSTD(1)),
    `Events.Timestamp` Array(DateTime64(9)) CODEC(ZSTD(1)),
    `Events.Name` Array(LowCardinality(String)) CODEC(ZSTD(1)),
    `Events.Attributes` Array(Map(LowCardinality(String), String)) CODEC(ZSTD(1)),
    `Links.TraceId` Array(String) CODEC(ZSTD(1)),
    `Links.SpanId` Array(String) CODEC(ZSTD(1)),
    `Links.TraceState` Array(String) CODEC(ZSTD(1)),
    `Links.Attributes` Array(Map(LowCardinality(String), String)) CODEC(ZSTD(1)),
	
	`Resource.service.namespace` String CODEC(ZSTD(1)),	
	`Resource.service.version` String CODEC(ZSTD(1)),	
	`Resource.service.instance.id` String CODEC(ZSTD(1)),
	
	`Attributes.http.status_code` String CODEC(ZSTD(1)),
	`Attributes.http.response_content_body` String CODEC(ZSTD(1)),
	`Attributes.http.request_content_body` String CODEC(ZSTD(1)),
	`Attributes.http.target` String CODEC(ZSTD(1)),
	`Attributes.exception.message` String CODEC(ZSTD(1)),

    `ResourceAttributesKeys` Array(String) CODEC(ZSTD(1)),
    `ResourceAttributesValues` Array(String) CODEC(ZSTD(1)),
    `SpanAttributesKeys` Array(String) CODEC(ZSTD(1)),
    `SpanAttributesValues` Array(String) CODEC(ZSTD(1)),

    INDEX idx_trace_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_servicenamespace Resource.service.namespace TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_serviceinstanceid Resource.service.instance.id TYPE bloom_filter(0.001) GRANULARITY 1,
	INDEX idx_trace_statuscode Attributes.http.status_code TYPE bloom_filter(0.001) GRANULARITY 1,	
	
	INDEX idx_string_requestbody Attributes.http.request_content_body TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1,
	INDEX idx_string_responsebody Attributes.http.response_content_body TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1,
	INDEX idx_string_exceptionmessage Attributes.exception.message TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1
)
ENGINE = MergeTree
PARTITION BY toDate(Timestamp)
ORDER BY (
 Timestamp,
 Resource.service.namespace,
 ServiceName
 )
TTL toDateTime(Timestamp) + toIntervalDay(30)
SETTINGS index_granularity = 8192,
 ttl_only_drop_parts = 1;
",
$@"CREATE MATERIALIZED VIEW {MasaStackClickhouseConnection.LogTable}_v TO {MasaStackClickhouseConnection.LogTable}
AS
SELECT
Timestamp,TraceId,SpanId,TraceFlags,SeverityText,SeverityNumber,ServiceName,Body,ResourceSchemaUrl,toJSONString(ResourceAttributes) as Resources,
ScopeSchemaUrl,ScopeName,ScopeVersion,toJSONString(ScopeAttributes) as Scopes,toJSONString(LogAttributes) as Logs,
ResourceAttributes['service.namespace'] as `Resource.service.namespace`,ResourceAttributes['service.version'] as `Resource.service.version`,
ResourceAttributes['service.instance.id'] as `Resource.service.instance.id`,
LogAttributes['TaskId'] as `Attributes.TaskId`,LogAttributes['exception.message'] as `Attributes.exception.message`,
mapKeys(ResourceAttributes) as ResourceAttributesKeys,mapValues(ResourceAttributes) as ResourceAttributesValues,
mapKeys(LogAttributes) as LogAttributesKeys,mapValues(LogAttributes) as LogAttributesValues
FROM {MasaStackClickhouseConnection.LogSourceTable}
",
$@"CREATE MATERIALIZED VIEW {MasaStackClickhouseConnection.TraceTable}_v TO {MasaStackClickhouseConnection.TraceTable}
AS
SELECT
    Timestamp,TraceId,SpanId,ParentSpanId,TraceState,SpanName,SpanKind,ServiceName,toJSONString(ResourceAttributes) AS Resources,
    ScopeName,ScopeVersion,toJSONString(SpanAttributes) AS Spans,
    Duration,StatusCode,StatusMessage,Events.Timestamp,Events.Name,Events.Attributes,
    Links.TraceId,Links.SpanId,Links.TraceState,Links.Attributes,
    
    ResourceAttributes['service.namespace'] as `Resource.service.namespace`,ResourceAttributes['service.version'] as `Resource.service.version`,
    ResourceAttributes['service.instance.id'] as `Resource.service.instance.id`,
    
    SpanAttributes['http.status_code'] as `Attributes.http.status_code`,
    SpanAttributes['http.response_content_body'] as `Attributes.http.response_content_body`,
    SpanAttributes['http.request_content_body'] as `Attributes.http.request_content_body`,
    SpanAttributes['http.target'] as `Attributes.http.target`,
    SpanAttributes['exception.message'] as `Attributes.exception.message`,   

    mapKeys(ResourceAttributes) AS ResourceAttributesKeys,
    mapValues(ResourceAttributes) AS ResourceAttributesValues,
    mapKeys(SpanAttributes) AS SpanAttributesKeys,
    mapValues(SpanAttributes) AS SpanAttributesValues
FROM {MasaStackClickhouseConnection.TraceSourceTable}
" };

        foreach (var sql in createTableSqls)
        {
            ExecuteSql(connection, sql);
        }
    }

    private static void InitMappingTable(MasaStackClickhouseConnection connection)
    {
        var database = connection.ConnectionSettings?.Database;
        database ??= new ClickHouseConnectionSettings(connection.ConnectionString).Database;

        var list = MasaStackClickhouseConnection.MappingTable.Split('.');
        var mappingTable = list[list.Length - 1];
        if (Convert.ToInt32(connection.ExecuteScalar($"select count() from system.tables where database ='{database}' and name in ['{mappingTable}']")) > 0)
            return;

        var initSqls = new string[]{
$@"
CREATE TABLE {database}.otel_mapping
(
    `Name` Array(String),
    `Type` String
)
ENGINE = MergeTree
ORDER BY Name
SETTINGS index_granularity = 8192;",
@$"CREATE MATERIALIZED VIEW {database}.v_otel_traces_attribute_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(SpanAttributes)) as Name, 'trace_attributes' as Type
from {MasaStackClickhouseConnection.TraceSourceTable}",
$@"CREATE MATERIALIZED VIEW {database}.v_otel_traces_resource_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(ResourceAttributes)) as Name, 'trace_resource' as Type
from {MasaStackClickhouseConnection.TraceSourceTable}",
$@"CREATE MATERIALIZED VIEW {database}.v_otel_logs_attribute_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(LogAttributes)) as Name, 'log_attributes' as Type
from {MasaStackClickhouseConnection.LogSourceTable}",
$@"CREATE MATERIALIZED VIEW {database}.v_otel_logs_resource_mapping to {MasaStackClickhouseConnection.MappingTable}
as
select DISTINCT arraySort(mapKeys(ResourceAttributes)) as Name, 'log_resource' as Type
from {MasaStackClickhouseConnection.LogSourceTable}",
$@"insert into {MasaStackClickhouseConnection.MappingTable}
values (['Timestamp','TraceId','SpanId','TraceFlag','SeverityText','SeverityNumber','Body'],'log_basic'),
(['Timestamp','TraceId','SpanId','ParentSpanId','TraceState','SpanKind','Duration'],'trace_basic');
" };
        foreach (var sql in initSqls)
            connection.ExecuteSql(sql);
    }

    internal static void ExecuteSql(this IDbConnection connection, string sql)
    {
        using var cmd = connection.CreateCommand();
        if (connection.State != ConnectionState.Open)
            connection.Open();
        cmd.CommandText = sql;
        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "ExecuteSql {rawSql} error", sql);
        }
    }

    private static string GetTimezone(MasaStackClickhouseConnection connection)
    {
        using var cmd = connection.CreateCommand();
        if (connection.State != ConnectionState.Open)
            connection.Open();
        var sql = "select timezone()";
        cmd.CommandText = sql;
        try
        {
            return cmd.ExecuteScalar()?.ToString()!;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "ExecuteSql {rawSql} error", sql);
            throw;
        }
    }
}
