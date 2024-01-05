// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse.Tests;

public class Common
{
    public static void InitTable(bool isLog, IDbConnection connection)
    {
        var name = isLog ? "log" : "trace";
        if (connection.State == ConnectionState.Closed)
            connection.Open();
        using var cmd = connection.CreateCommand();
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data/otel_{name}.txt");
        using var reader = new StreamReader(path);

        var sql = reader.ReadToEnd();
        cmd.CommandText = sql;
        try
        {
            cmd.ExecuteNonQuery();
        }
        catch
        {
            //table is exists
        }

    }

    public static void InitTableData(bool isLog, string rootPath, IDbConnection connection)
    {
        var name = isLog ? "log" : "trace";
        if (connection.State == ConnectionState.Closed)
            connection.Open();
        using var cmd = connection.CreateCommand();
        var path = Path.Combine(rootPath, $"Data/otel_{name}_data.txt");
        using var dataReader = new StreamReader(path);
        var sql = dataReader.ReadToEnd();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public static void InitTableJsonData(bool isLog, string rootPath, IDbConnection connection)
    {
        var name = isLog ? "log" : "trace";
        if (connection.State == ConnectionState.Closed)
            connection.Open();
        using var cmd = connection.CreateCommand();
        var path = Path.Combine(rootPath, $"Data/otel_{name}_data.json");
        using var dataReader = new StreamReader(path);
        var data = dataReader.ReadToEnd();
        var sql = GetInsertSql(data, $"otel_{name}s");
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private static string? GetInsertSql(string jsonData, string table = "ttttttt")
    {
        var data = JsonSerializer.Deserialize<JsonElement[]>(jsonData);
        if (data == null || data.Length == 0)
            return default;

        var header = new StringBuilder($"insert into {table}");
        header.Append('(').AppendLine();
        bool isFirst = true;
        var sql = new StringBuilder();
        foreach (var jsonNode in data)
        {
            sql.Append('(');
            foreach (var item in jsonNode.EnumerateObject())
            {
                if (isFirst)
                {
                    header.Append($"{item.Name},");
                }
                if (item.Value.ValueKind == JsonValueKind.Number)
                    sql.Append($"{item.Value},");
                else if (item.Value.TryGetDateTime(out var time))
                    sql.Append($"'{time.ToString("yyyy-MM-dd HH:mm:ss.ffff")}',");
                else
                    sql.Append($"'{item.Value.ToString().Replace("\\'", "''").Replace("'", "''").Replace("@","")}',");
            }
            sql.Remove(sql.Length - 1, 1).AppendLine("),");
            isFirst = false;
        }
        header.Remove(header.Length - 1, 1).Append(')').AppendLine().Append("values").AppendLine().Append(sql.Remove(sql.Length - 1, 1));
        return header.ToString();
    }
}
