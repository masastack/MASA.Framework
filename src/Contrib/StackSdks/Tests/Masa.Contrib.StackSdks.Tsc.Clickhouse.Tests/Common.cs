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
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data/otel_{name}.txt");
        using (var reader = new StreamReader(path))
        {
            var sql = reader.ReadToEnd();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data/otel_{name}_data.txt");
        using (var dataReader = new StreamReader(path))
        {
            var sql = dataReader.ReadToEnd();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
