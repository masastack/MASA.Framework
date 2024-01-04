// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Config;

internal static class Constants
{
    public static string Database { get; private set; }

    public static string TraceTableFull => $"{Database}.{TraceTable}";

    public static string ErrorTableFull => $"{Database}.{ErrorTable}";

    public static string LogTableFull => $"{Database}.{LogTable}";

    public static string LogTable { get; private set; }

    public static string TraceTable { get; private set; }

    public static string ErrorTable { get; private set; }

    public static void Init(string database, string logTable, string traceTable, string errorTable)
    {
        Database = database;
        LogTable = logTable;
        TraceTable = traceTable;
        ErrorTable = errorTable;
    }

    public static int[] DefaultErrorStatus = new int[] { 500, 501, 502, 503, 504, 505 };
}
