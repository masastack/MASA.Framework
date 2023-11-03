// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse.Tests;

internal class Common
{
    public static void InitTableData(bool isLog)
    {
        var name = isLog ? "log" : "trace";
        using var connection = new ClickHouseConnection(Consts.ConnectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();
        using (var reader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}Data\\otel_{name}.txt"))
        {
            var sql = reader.ReadToEnd();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        using (var dataReader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}Data\\otel_{name}_data.txt"))
        {
            var sql = dataReader.ReadToEnd();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
