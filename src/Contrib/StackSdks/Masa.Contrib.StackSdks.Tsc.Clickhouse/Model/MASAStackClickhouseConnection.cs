// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.
[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse")]
[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Tests")]
namespace Masa.Contrib.StackSdks.Tsc.Clickhouse;

internal sealed class MasaStackClickhouseConnection : ClickHouseConnection
{
    public static string LogSourceTable { get; private set; }

    public static string TraceSourceTable { get; private set; }

    public static string LogTable { get; private set; }

    public static string TraceTable { get; private set; }

    public static string MappingTable { get; private set; }

    public static TimeZoneInfo TimeZone { get; set; }

    public static DateTime ToTimeZone(DateTime utcTime)
    {
        return utcTime + TimeZone.BaseUtcOffset;
    }

    public MasaStackClickhouseConnection(string connection, string logTable, string traceTable, string? logSourceTable = null, string? traceSourceTable = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(logTable);
        ArgumentNullException.ThrowIfNull(traceTable);
        ConnectionString = connection;
        logSourceTable ??= "otel_logs";
        traceSourceTable ??= "otel_traces";

        if (!string.IsNullOrEmpty(ConnectionSettings.Database))
        {
            LogTable = $"{ConnectionSettings.Database}.{logTable}";
            TraceTable = $"{ConnectionSettings.Database}.{traceTable}";
            TraceSourceTable = $"{ConnectionSettings.Database}.{traceSourceTable}";
            LogSourceTable = $"{ConnectionSettings.Database}.{logSourceTable}";
            MappingTable = $"{ConnectionSettings.Database}.otel_mapping";
        }
        else
        {
            LogTable = logTable;
            TraceTable = traceTable;
            TraceSourceTable = traceSourceTable;
            LogSourceTable = logSourceTable;
            MappingTable = "otel_mapping";
        }
    }
}
