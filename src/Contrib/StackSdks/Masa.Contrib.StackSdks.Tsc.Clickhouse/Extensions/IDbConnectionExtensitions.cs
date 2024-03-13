// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.
[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse")]
namespace System.Data.Common;

internal static class IDbConnectionExtensitions
{
    const string ATTRIBUTE_KEY = "Attributes.";
    const string RESOURCE_KEY = "Resource.";
    const string TIMSTAMP_KEY = "Timestamp";

    public static PaginatedListBase<TraceResponseDto> QueryTrace(this IDbConnection connection, BaseRequestDto query)
    {
        var (where, parameters, ors) = AppendWhere(query);
        var orderBy = AppendOrderBy(query, false);
        var countSql = CombineOrs($"select count() as `total` from {MasaStackClickhouseConnection.TraceTable} where {where}", ors);
        var total = Convert.ToInt64(ExecuteScalar(connection, $"select sum(`total`) from {countSql}", parameters?.ToArray()));
        var start = (query.Page - 1) * query.PageSize;
        var result = new PaginatedListBase<TraceResponseDto>() { Total = total, Result = new() };
        if (total > 0 && start - total < 0)
        {
            var querySql = CombineOrs($"select ServiceName,{TIMSTAMP_KEY},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceTable} where {where}", ors, orderBy);
            result.Result = Query(connection, $"select * from {querySql} as t limit {start},{query.PageSize}", parameters?.ToArray(), ConvertTraceDto);
        }
        return result;
    }

    public static PaginatedListBase<LogResponseDto> QueryLog(this IDbConnection connection, BaseRequestDto query)
    {
        var (where, parameters, ors) = AppendWhere(query, false);
        var orderBy = AppendOrderBy(query, true);
        var countSql = CombineOrs($"select count() as `total` from {MasaStackClickhouseConnection.LogTable} where {where}", ors);
        var total = Convert.ToInt64(ExecuteScalar(connection, $"select sum(`total`) from {countSql}", parameters?.ToArray()));
        var start = (query.Page - 1) * query.PageSize;
        var result = new PaginatedListBase<LogResponseDto>() { Total = total, Result = new() };


        if (total > 0 && start - total < 0)
        {
            var querySql = CombineOrs($"select {TIMSTAMP_KEY},TraceId,SpanId,TraceFlags,SeverityText,SeverityNumber,ServiceName,Body,Resources,Logs from {MasaStackClickhouseConnection.LogTable} where {where}", ors, orderBy);
            result.Result = Query(connection, $"select * from {querySql} as t limit {start},{query.PageSize}", parameters?.ToArray(), ConvertLogDto);
        }
        return result;
    }

    private static string CombineOrs(string sql, IEnumerable<string> ors, string? orderBy = null)
    {
        if (ors == null || !ors.Any())
            return $"({sql} {orderBy})";

        var text = new StringBuilder();
        foreach (var or in ors)
        {
            text.AppendLine($" union all {sql}{or} {orderBy}");
        }
        text.Remove(0, 11).Insert(0, '(').Append(')');
        return text.ToString();
    }

    public static List<MappingResponseDto> GetMapping(this IDbConnection dbConnection, bool isLog)
    {
        var type = isLog ? "log" : "trace";
        var result = dbConnection.Query($"select DISTINCT Name from otel_mapping Array join Name where `Type`='{type}_basic' order by Name", default, ConvertToMapping);
        if (result == null || result.Count == 0)
            return default!;

        var attributes = dbConnection.Query($"select DISTINCT concat('{ATTRIBUTE_KEY}',Name)  from otel_mapping Array join Name where `Type`='{type}_attributes' order by Name", default, ConvertToMapping);
        var resources = dbConnection.Query($"select DISTINCT concat('{RESOURCE_KEY}',Name)  from otel_mapping Array join Name where `Type`='resource' order by Name", default, ConvertToMapping);
        if (attributes != null && attributes.Count > 0) result.AddRange(attributes);
        if (resources != null && resources.Count > 0) result.AddRange(resources);

        return result;
    }

    public static List<TraceResponseDto> GetTraceByTraceId(this IDbConnection connection, string traceId)
    {
        string where = $"TraceId=@TraceId";
        return Query(connection, $"select * from (select {TIMSTAMP_KEY},TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,Spans,Resources from {MasaStackClickhouseConnection.TraceTable} where {where}) as t limit 1000", new IDataParameter[] { new ClickHouseParameter { ParameterName = "TraceId", Value = traceId } }, ConvertTraceDto);
    }

    public static string AppendOrderBy(BaseRequestDto query, bool isLog)
    {
        var field = TIMSTAMP_KEY;
        var isDesc = query.Sort?.IsDesc ?? true;
        if (isLog && query.Sort != null && !string.IsNullOrEmpty(query.Sort.Name))
        {
            field = GetName(query.Sort.Name, isLog);
            isDesc = query.Sort?.IsDesc ?? false;
        }
        return $" order by {field}{(isDesc ? " desc" : "")}";
    }

    public static (string where, List<IDataParameter> @parameters, List<string> ors) AppendWhere(BaseRequestDto query, bool isTrace = true)
    {
        var sql = new StringBuilder();
        var @paramerters = new List<IDataParameter>();

        if (query.Start > DateTime.MinValue && query.Start < DateTime.MaxValue
            && query.End > DateTime.MinValue && query.End < DateTime.MaxValue
            && query.End > query.Start)
        {
            sql.Append($" and {TIMSTAMP_KEY} BETWEEN @Start and @End");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "Start", Value = MasaStackClickhouseConnection.ToTimeZone(query.Start), DbType = DbType.DateTime2 });
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "End", Value = MasaStackClickhouseConnection.ToTimeZone(query.End), DbType = DbType.DateTime2 });
        }
        if (!string.IsNullOrEmpty(query.Service))
        {
            sql.Append(" and ServiceName=@ServiceName");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "ServiceName", Value = query.Service });
        }
        if (!string.IsNullOrEmpty(query.Instance))
        {
            sql.Append($" and `{RESOURCE_KEY}service.instance.id`=@ServiceInstanceId");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "ServiceInstanceId", Value = query.Instance });
        }
        if (isTrace && !string.IsNullOrEmpty(query.Endpoint))
        {
            sql.Append($" and `{ATTRIBUTE_KEY}http.target`=@HttpTarget");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "HttpTarget", Value = query.Endpoint });
        }
        var ors = AppendKeyword(query.Keyword, paramerters, isTrace);
        AppendConditions(query.Conditions, paramerters, sql, isTrace);

        if (!string.IsNullOrEmpty(query.RawQuery))
            sql.Append($" and ({query.RawQuery})");

        if (sql.Length > 0)
            sql.Remove(0, 4);
        return (sql.ToString(), @paramerters, ors);
    }

    private static List<string> AppendKeyword(string keyword, List<IDataParameter> @paramerters, bool isTrace = true)
    {
        var sqls = new List<string>();
        if (string.IsNullOrEmpty(keyword))
            return sqls;

        //status_code
        if (int.TryParse(keyword, out var num) && num != 0 && num - 1000 < 0 && isTrace)
        {
            sqls.Add($" and `{ATTRIBUTE_KEY}http.status_code`=@HttpStatusCode");
            sqls.Add($" and `{ATTRIBUTE_KEY}http.request_content_body` like @Keyword");
            paramerters.Add(new ClickHouseParameter() { ParameterName = "HttpStatusCode", Value = num });
            paramerters.Add(new ClickHouseParameter() { ParameterName = "Keyword", Value = $"%{keyword}%" });
            return sqls;
        }

        if (isTrace)
        {
            sqls.Add($" and `{ATTRIBUTE_KEY}http.request_content_body` like @Keyword");
            sqls.Add($" and `{ATTRIBUTE_KEY}http.response_content_body` like @Keyword");
            sqls.Add($" and `{ATTRIBUTE_KEY}exception.message` like @Keyword");
        }
        else
        {
            if (keyword.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                sqls.Add(" and SeverityText='Error'");
            sqls.Add(" and Body like @Keyword");
            sqls.Add($" and `{ATTRIBUTE_KEY}exception.message` like @Keyword");
        }
        paramerters.Add(new ClickHouseParameter() { ParameterName = "Keyword", Value = $"%{keyword}%" });
        return sqls;
    }

    private static void AppendConditions(IEnumerable<FieldConditionDto>? conditions, List<IDataParameter> @paramerters, StringBuilder sql, bool isTrace = true)
    {
        if (conditions == null || !conditions.Any())
            return;

        foreach (var item in conditions)
        {
            var name = GetName(item.Name, !isTrace);

            if (item.Value is DateTime time)
            {
                item.Value = MasaStackClickhouseConnection.ToTimeZone(time);
            }
            if (item.Name.StartsWith(RESOURCE_KEY, StringComparison.CurrentCultureIgnoreCase))
            {
                var filed = item.Name[RESOURCE_KEY.Length..];
                if (string.Equals(filed, "service.name"))
                {
                    AppendField(item, @paramerters, sql, name, "ServiceName");
                }
                else if (string.Equals(filed, "service.instance.id"))
                {
                    AppendField(item, @paramerters, sql, name, "ServiceInstanceId");
                }
                else if (string.Equals(filed, "service.namespace"))
                {
                    AppendField(item, @paramerters, sql, name, "ServiceNameSpace");
                }
            }
            else if (item.Name.StartsWith(ATTRIBUTE_KEY, StringComparison.CurrentCultureIgnoreCase))
            {
                var filed = item.Name[ATTRIBUTE_KEY.Length..];
                AppendField(item, @paramerters, sql, name, filed.Replace('.', '_'));
            }
            else
            {
                AppendField(item, @paramerters, sql, name, name);
            }
        }
    }

    private static void AppendField(FieldConditionDto item, List<IDataParameter> @paramerters, StringBuilder sql, string fieldName, string paramName)
    {
        if (item.Value is string str && string.IsNullOrEmpty(str) || item.Value is IEnumerable<object> collects && !collects.Any())
            return;
        switch (item.Type)
        {
            case ConditionTypes.Equal:
                {
                    if (@paramerters.Exists(p => p.ParameterName == paramName))
                        break;
                    ParseWhere(sql, item.Value, paramerters, fieldName, paramName, "=");
                }
                break;
            case ConditionTypes.NotIn:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"{paramName}s", "not in");
                }
                break;
            case ConditionTypes.In:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"{paramName}s", "in");
                }
                break;
            case ConditionTypes.LessEqual:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"lte_{paramName}", "<=");
                }
                break;
            case ConditionTypes.GreatEqual:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"gte_{paramName}", ">=");
                }
                break;
            case ConditionTypes.Less:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"lt_{paramName}", "<");
                }
                break;
            case ConditionTypes.Great:
                {
                    ParseWhere(sql, item.Value, paramerters, fieldName, $"gt_{paramName}", ">");
                }
                break;
        }
    }

    private static void ParseWhere(StringBuilder sql, object value, List<IDataParameter> @paramerters, string fieldName, string paramName, string compare)
    {
        DbType dbType = value is DateTime ? DbType.DateTime2 : DbType.AnsiString;
        if (value is IEnumerable)
            sql.Append($" and {fieldName} {compare} (@{paramName})");
        else
            sql.Append($" and {fieldName} {compare} @{paramName}");
        @paramerters.Add(new ClickHouseParameter { ParameterName = $"{paramName}", Value = value, DbType = dbType });
    }

    public static object? ExecuteScalar(this IDbConnection dbConnection, string sql, IDataParameter[]? @parameters = null)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = sql;
        if (@parameters != null && @parameters.Length > 0)
            foreach (var p in @parameters)
                cmd.Parameters.Add(p);
        OpenConnection(dbConnection);
        try
        {
            return cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            MasaTscCliclhouseExtensitions.Logger?.LogError(ex, "execute sql error:{RawSql}, paramters:{Parameters}", sql, parameters);
            throw;
        }
    }

    private static void OpenConnection(IDbConnection dbConnection)
    {
        if (dbConnection.State == ConnectionState.Closed)
            dbConnection.Open();
    }

    public static List<T> Query<T>(this IDbConnection dbConnection, string sql, IDataParameter[]? @parameters, Func<IDataReader, T> parse)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = sql;
        if (@parameters != null && @parameters.Length > 0)
            foreach (var p in @parameters)
                cmd.Parameters.Add(p);
        OpenConnection(dbConnection);
        try
        {
            using var reader = cmd.ExecuteReader();
            if (reader == null)
                return new List<T>();
            var list = new List<T>();
            while (reader.NextResult())
                while (reader.Read())
                {
                    list.Add(parse.Invoke(reader));
                }
            return list;
        }
        catch (Exception ex)
        {
            MasaTscCliclhouseExtensitions.Logger?.LogError(ex, "query sql error:{RawSql}, paramters:{Parameters}", sql, parameters);
            throw;
        }
    }

    public static MappingResponseDto ConvertToMapping(IDataReader reader)
    {
        return new MappingResponseDto
        {
            Name = reader[0].ToString()!,
            Type = "string"
        };
    }

    public static TraceResponseDto ConvertTraceDto(IDataReader reader)
    {
        var startTime = Convert.ToDateTime(reader[TIMSTAMP_KEY]);
        long ns = Convert.ToInt64(reader["Duration"]);
        string resource = reader["Resources"].ToString()!, spans = reader["Spans"].ToString()!;
        var result = new TraceResponseDto
        {
            TraceId = reader["TraceId"].ToString()!,
            EndTimestamp = startTime.AddMilliseconds(ns / 1e6),
            Kind = reader["SpanKind"].ToString()!,
            Name = reader["SpanName"].ToString()!,
            ParentSpanId = reader["ParentSpanId"].ToString()!,
            SpanId = reader["SpanId"].ToString()!,
            Timestamp = startTime
        };
        if (!string.IsNullOrEmpty(resource))
            result.Resource = JsonSerializer.Deserialize<Dictionary<string, object>>(resource)!;
        if (!string.IsNullOrEmpty(spans))
            result.Attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(spans)!;
        return result;
    }

    public static LogResponseDto ConvertLogDto(IDataReader reader)
    {
        string resource = reader["Resources"].ToString()!, logs = reader["Logs"].ToString()!;
        var result = new LogResponseDto
        {
            TraceId = reader["TraceId"].ToString()!,
            Body = reader["Body"].ToString()!,
            SeverityNumber = Convert.ToInt32(reader["SeverityNumber"]),
            SeverityText = reader["SeverityText"].ToString()!,
            TraceFlags = Convert.ToInt32(reader["TraceFlags"]),
            SpanId = reader["SpanId"].ToString()!,
            Timestamp = Convert.ToDateTime(reader[TIMSTAMP_KEY]),
        };
        if (!string.IsNullOrEmpty(resource))
            result.Resource = JsonSerializer.Deserialize<Dictionary<string, object>>(resource)!;
        if (!string.IsNullOrEmpty(logs))
            result.Attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(logs)!;
        return result;
    }

    public static object AggregationQuery(this IDbConnection dbConnection, SimpleAggregateRequestDto requestDto, bool isLog = true)
    {
        var sql = new StringBuilder("select ");
        var append = new StringBuilder();
        var appendWhere = new StringBuilder();
        var name = GetName(requestDto.Name, isLog);
        AppendAggtype(requestDto, sql, append, name, out var isScalar);
        sql.AppendFormat(" from {0} ", isLog ? MasaStackClickhouseConnection.LogTable : MasaStackClickhouseConnection.TraceTable);
        var (where, @paremeters, _) = AppendWhere(requestDto, !isLog);
        sql.Append($" where {appendWhere} {where}");
        sql.Append(append);
        var paramArray = @paremeters?.ToArray()!;

        if (isScalar)
        {
            return dbConnection.ExecuteScalar(sql.ToString(), paramArray)!;
        }
        else
        {
            return AggTerm(dbConnection, sql.ToString(), paramArray, requestDto.Type, requestDto.AllValue);
        }
    }

    private static object AggTerm(IDbConnection dbConnection, string sql, IDataParameter[] paramArray, AggregateTypes aggregateTypes, bool isAllValue)
    {
        var result = dbConnection.Query(sql, paramArray, reader =>
        {
            if (aggregateTypes == AggregateTypes.GroupBy)
            {
                if (isAllValue)
                    return KeyValuePair.Create(reader[0].ToString(), Convert.ToInt64(reader[1]));
                else
                    return reader[0];
            }
            else
            {
                var time = Convert.ToDateTime(reader[0]);
                var timestamp = new DateTimeOffset(time).ToUnixTimeMilliseconds();
                return KeyValuePair.Create(timestamp, Convert.ToInt64(reader[1]));
            }
        });
        if (aggregateTypes == AggregateTypes.GroupBy)
        {
            if (isAllValue)
                return result.Select(item => (KeyValuePair<string, long>)item).ToList();
            else
                return result.Select(item => item.ToString()).ToList();
        }
        return result;
    }

    private static void AppendAggtype(SimpleAggregateRequestDto requestDto, StringBuilder sql, StringBuilder append, string name, out bool isScalar)
    {
        isScalar = false;
        switch (requestDto.Type)
        {
            case AggregateTypes.Avg:
                sql.Append($"AVG({name}) as a");
                isScalar = true;
                break;
            case AggregateTypes.Count:
                sql.Append($"Count({name})  as a");
                isScalar = true;
                break;
            case AggregateTypes.DistinctCount:
                sql.Append($"Count(DISTINCT {name})  as a");
                isScalar = true;
                break;
            case AggregateTypes.Sum:
                sql.Append($"SUM({name})  as a");
                isScalar = true;
                break;
            case AggregateTypes.GroupBy:
                sql.Append($"{name} as a,Count({name})  as b");
                append.Append($" and a<>'' Group By a order by b desc");
                break;
            case AggregateTypes.DateHistogram:
                sql.Append($"toStartOfInterval({name}, INTERVAL {ConvertInterval(requestDto.Interval)} minute ) as `time`,count() as `count`");
                append.Append($" Group by `time` order by `time`");
                break;
        }
    }

    private static string GetName(string name, bool isLog)
    {
        if (name.Equals("@timestamp", StringComparison.CurrentCultureIgnoreCase))
            return TIMSTAMP_KEY;

        if (!isLog && name.Equals("duration", StringComparison.CurrentCultureIgnoreCase))
            return "Duration";

        if (!isLog && name.Equals("kind", StringComparison.InvariantCultureIgnoreCase))
            return "SpanKind";

        if (name.StartsWith(RESOURCE_KEY, StringComparison.CurrentCultureIgnoreCase))
            return GetResourceName(name);

        if (name.StartsWith(ATTRIBUTE_KEY, StringComparison.CurrentCultureIgnoreCase))
            return GetAttributeName(name, isLog);

        return name;
    }

    private static string GetResourceName(string name)
    {
        var field = name[(RESOURCE_KEY.Length)..];
        if (field.Equals("service.name", StringComparison.CurrentCultureIgnoreCase))
            return "ServiceName";

        if (field.Equals("service.namespace", StringComparison.CurrentCultureIgnoreCase) || field.Equals("service.instance.id", StringComparison.CurrentCultureIgnoreCase))
            return $"{RESOURCE_KEY}{field}";

        return $"ResourceAttributesValues[indexOf(ResourceAttributesKeys,'{field}')]";
    }

    private static string GetAttributeName(string name, bool isLog)
    {
        var pre = isLog ? "Log" : "Span";
        var field = name[(ATTRIBUTE_KEY.Length)..];
        if (isLog && (field.Equals("exception.message", StringComparison.CurrentCultureIgnoreCase)))
            return $"{ATTRIBUTE_KEY}{field}";

        if (!isLog && (field.Equals("http.status_code", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("http.request_content_body", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("http.response_content_body", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("exception.message", StringComparison.CurrentCultureIgnoreCase)
            || field.Equals("http.target", StringComparison.CurrentCultureIgnoreCase))
            )
            return $"{ATTRIBUTE_KEY}{field}";

        return $"{pre}AttributesValues[indexOf({pre}AttributesKeys,'{field}')]";
    }

    public static int ConvertInterval(string s)
    {
        var unit = Regex.Replace(s, @"\d+", "", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
        int t = 1;
        switch (unit)
        {
            case "s":
                t = 1;
                break;
            case "m":
                t = 60;
                break;
            case "h":
                t = 3600;
                break;
            case "d":
                t = 3600 * 24;
                break;
            case "w":
                t = 3600 * 24 * 7;
                break;
            case "month":
                t = 3600 * 24 * 30;
                break;
        }
        var num = Convert.ToInt64(s.Replace(unit, ""));
        num *= t;
        if (num - 60 < 0)
            return 1;
        return (int)(num / 60);
    }

    public static string GetMaxDelayTraceId(this IDbConnection dbConnection, BaseRequestDto requestDto)
    {
        var (where, parameters, _) = AppendWhere(requestDto);
        var text = $"select * from( TraceId from {MasaStackClickhouseConnection.TraceTable} where {where} order by Duration desc) as t limit 1";
        return dbConnection.ExecuteScalar(text, parameters?.ToArray())?.ToString()!;
    }
}
