// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ApmClickhouseServiceExtensions
{
    internal static ILogger Logger { get; private set; }

    public static IServiceCollection AddMASAStackApmClickhouse(this IServiceCollection services, string connectionStr, string logTable, string traceTable, string? logSourceTable = null, string? traceSourceTable = null, Action<IDbConnection>? configer = null)
    {
        return services.AddMASAStackClickhouse(connectionStr, logTable, traceTable, logSourceTable, traceSourceTable, con =>
         {
             Constants.Init(MasaStackClickhouseConnection.LogTable.Split('.')[0], MasaStackClickhouseConnection.LogTable.Split('.')[1], MasaStackClickhouseConnection.TraceTable.Split('.')[1], "otel_errors");
             services.TryAddScoped<IApmService>(builder => new ClickhouseApmService(con, services.BuildServiceProvider().GetRequiredService<ITraceService>()));
             Init(services, con);
             configer?.Invoke(con);

         });
    }

    private static void Init(IServiceCollection services, IDbConnection connection)
    {
        var serviceProvider = services.BuildServiceProvider();
        var logfactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        Logger = logfactory.CreateLogger("Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse");
        InitTable(connection);
    }

    private static void InitTable(IDbConnection connection)
    {
        if (Convert.ToInt32(connection.ExecuteScalar($"select count() from system.tables where database ='{Constants.Database}' and name in ['{Constants.ErrorTable}','{Constants.ErrorTable}_v']")) > 0)
            return;
        var createTableSqls = new string[]{
            @$"CREATE TABLE {Constants.Database}.{Constants.ErrorTable}
(
    `Timestamp` DateTime64(9) CODEC(Delta(8), ZSTD(1)),
    `TraceId` String CODEC(ZSTD(1)),
    `SpanId` String CODEC(ZSTD(1)),
    `Attributes.exception.message` String CODEC(ZSTD(1)),
    `Attributes.exception.type` String CODEC(ZSTD(1)),
    `ServiceName` String CODEC(ZSTD(1)),
    `Resource.service.namespace` String CODEC(ZSTD(1)),
    `Attributes.http.target` String CODEC(ZSTD(1)),
    INDEX idx_log_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_log_spanid SpanId TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_log_environment `Resource.service.namespace` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_log_servicename ServiceName TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_log_type `Attributes.exception.type` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_log_endpoint `Attributes.http.target` TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_string_message `Attributes.exception.message` TYPE tokenbf_v1(30720, 2, 0) GRANULARITY 1
)
ENGINE = MergeTree
PARTITION BY toDate(Timestamp)
ORDER BY (Timestamp,
 ServiceName,
 `Resource.service.namespace`,
 `Attributes.exception.type`,
`Attributes.http.target`)
TTL toDateTime(Timestamp) + toIntervalDay(30)
SETTINGS index_granularity = 8192,
 ttl_only_drop_parts = 1;
",
$@"CREATE MATERIALIZED VIEW {Constants.Database}.{Constants.ErrorTable}_v TO {Constants.ErrorTableFull}
AS
SELECT
Timestamp,TraceId,SpanId, Body AS `Attributes.exception.message`,LogAttributes['exception.type'] AS `Attributes.exception.type`,
    ServiceName,ResourceAttributes['service.namespace'] AS `Resource.service.namespace`, LogAttributes['RequestPath'] AS `Attributes.http.target`
FROM {MasaStackClickhouseConnection.LogSourceTable}
WHERE mapContains(LogAttributes, 'exception.type')
"};
        foreach (var sql in createTableSqls)
        {
            connection.ExecuteSql(sql);
        }
    }
}
