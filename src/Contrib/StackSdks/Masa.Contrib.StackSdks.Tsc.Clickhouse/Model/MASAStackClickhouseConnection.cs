// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse;

internal class MasaStackClickhouseConnection : ClickHouseConnection
{
    public static string LogSourceTable { get; private set; }

    public static string TraceSourceTable { get; private set; }

    public static string LogTable { get; private set; }

    public static string TraceTable { get; private set; }

    public static string MappingTable { get; private set; }

    public MasaStackClickhouseConnection(string connection, string? logTable = null, string? traceTable = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ConnectionString = connection;
        logTable ??= "otel_logs";
        traceTable ??= "otel_traces";
        if (!string.IsNullOrEmpty(ConnectionSettings.Database))
        {
            LogTable = $"{ConnectionSettings.Database}.{logTable}";
            TraceTable = $"{ConnectionSettings.Database}.{traceTable}";
            MappingTable = $"{ConnectionSettings.Database}.otel_mapping";
        }
        else
        {
            LogTable = logTable;
            TraceTable = traceTable;
            MappingTable = "otel_mapping";
        }
        //Open();
        //ChangeDatabase(ConnectionSettings.Database);
        ////ConnectionTimeout = 5000;
        //JsonSerializerOptionsSetting.NumberHandling =
        //            JsonNumberHandling.AllowReadingFromString |
        //            JsonNumberHandling.WriteAsString;
    }

    public MasaStackClickhouseConnection(string connection, string logTable, string logSourceTable, string traceTable, string traceSourceTable)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(logTable);
        ArgumentNullException.ThrowIfNull(logSourceTable);
        ArgumentNullException.ThrowIfNull(traceTable);
        ArgumentNullException.ThrowIfNull(traceSourceTable);
        ConnectionString = connection;
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
