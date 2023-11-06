// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Elasticsearch.Net;
using Nest;

namespace System.Data.Common;

internal static class IDbConnectionExtensitions
{
    public static PaginatedListBase<TraceResponseDto> QueryTrace(this IDbConnection connection, BaseRequestDto query)
    {
        var sql = AppendWhere(query);
        var orderBy = AppendOrderBy(query, false);
        var total = Convert.ToInt64(ExecuteScalar(connection, $"select count(1) from {MasaStackClickhouseConnection.TraceTable} where {sql.where}", sql.parameters?.ToArray()));
        var start = (query.Page - 1) * query.PageSize;
        var result = new PaginatedListBase<TraceResponseDto>() { Total = total, Result = new() };
        if (total > 0 && start - total < 0)
        {
            result.Result = Query(connection, $"select * from(select ServiceName,Timestamp,TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,toJSONString(SpanAttributes) as Spans,toJSONString(ResourceAttributes) as Resources from {MasaStackClickhouseConnection.TraceTable} where {sql.where} {orderBy}) as t limit {start},{query.PageSize}", sql.parameters?.ToArray(), ConvertTraceDto);
        }
        return result;
    }

    public static PaginatedListBase<LogResponseDto> QueryLog(this IDbConnection connection, BaseRequestDto query)
    {
        var sql = AppendWhere(query, false);
        var orderBy = AppendOrderBy(query, true);
        var total = Convert.ToInt64(ExecuteScalar(connection, $"select count(1) from {MasaStackClickhouseConnection.LogTable} where {sql.where}", sql.parameters?.ToArray()));
        var start = (query.Page - 1) * query.PageSize;
        var result = new PaginatedListBase<LogResponseDto>() { Total = total, Result = new() };

        if (total > 0 && start - total < 0)
        {
            result.Result = Query(connection, $"select * from(select Timestamp,TraceId,SpanId,TraceFlags,SeverityText,SeverityNumber,ServiceName,Body,toJSONString(ResourceAttributes) as Resources,toJSONString(LogAttributes) as Logs from {MasaStackClickhouseConnection.LogTable} where {sql.where} {orderBy}) as t limit {start},{query.PageSize}", sql.parameters?.ToArray(), ConvertLogDto);
        }
        return result;
    }

    public static List<MappingResponseDto> GetMapping(this IDbConnection dbConnection, bool isLog)
    {
        var type = isLog ? "log" : "trace";
        var result = dbConnection.Query($"select DISTINCT Name from otel_mapping Array join Name where `Type`='{type}_basic' order by Name", default, ConvertToMapping);
        if (result == null || !result.Any())
            return default!;

        var attributes = dbConnection.Query($"select DISTINCT concat('Attributes.',Name)  from otel_mapping Array join Name where `Type`='{type}_attributes' order by Name", default, ConvertToMapping);
        var resources = dbConnection.Query("select DISTINCT concat('Resource.',Name)  from otel_mapping Array join Name where `Type`='resource' order by Name", default, ConvertToMapping);
        if (attributes != null && attributes.Any()) result.AddRange(attributes);
        if (resources != null && resources.Any()) result.AddRange(resources);

        return result;
    }

    public static List<TraceResponseDto> GetTraceByTraceId(this IDbConnection connection, string traceId)
    {
        string where = $"TraceId=@TraceId";
        return Query(connection, $"select * from (select Timestamp,TraceId,SpanId,ParentSpanId,TraceState,SpanKind,Duration,SpanName,toJSONString(SpanAttributes) as Spans,toJSONString(ResourceAttributes) as Resources from {MasaStackClickhouseConnection.TraceTable} where {where}) as t limit 1000", new IDataParameter[] { new ClickHouseParameter { ParameterName = "TraceId", Value = traceId } }, ConvertTraceDto);
    }

    public static string AppendOrderBy(BaseRequestDto query, bool isLog)
    {
        var str = query.Sort?.IsDesc ?? false ? " desc" : "";
        return $" order by Timestamp{str}";
    }

    public static (string where, List<IDataParameter> @parameters) AppendWhere(BaseRequestDto query, bool isTrace = true)
    {
        var sql = new StringBuilder();
        var @paramerters = new List<IDataParameter>();

        if (query.Start > DateTime.MinValue && query.Start < DateTime.MaxValue
            && query.End > DateTime.MinValue && query.End < DateTime.MaxValue
            && query.End > query.Start)
        {
            sql.Append($" and Timestamp BETWEEN @Start and @End");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "Start", Value = query.Start.ToLocalTime(), DbType = DbType.DateTime2 });
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "End", Value = query.End.ToLocalTime(), DbType = DbType.DateTime2 });
        }
        if (!string.IsNullOrEmpty(query.Service))
        {
            sql.Append(" and ServiceName=@ServiceName");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "ServiceName", Value = query.Service });
        }
        if (!string.IsNullOrEmpty(query.Instance))
        {
            sql.Append(" and ResourceAttributes['service.instance.id']=@ServiceInstanceId");
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "ServiceInstanceId", Value = query.Instance });
        }
        if (!string.IsNullOrEmpty(query.Endpoint))
        {
            if (isTrace)
            {
                sql.Append(" and SpanKind=@SpanKind and SpanAttributes['http.target']=@HttpTarget");
                @paramerters.Add(new ClickHouseParameter() { ParameterName = "SpanKind", Value = "SPAN_KIND_SERVER" });
            }
            else
            {
                sql.Append(" and mapContains(LogAttributes, 'Host') and LogAttributes['RequestPath']=@HttpTarget");
            }
            @paramerters.Add(new ClickHouseParameter() { ParameterName = "HttpTarget", Value = query.Instance });
        }
        AppendKeyword(query.Keyword, sql, paramerters, isTrace);
        AppendConditions(query.Conditions, paramerters, sql, isTrace);

        if (!string.IsNullOrEmpty(query.RawQuery))
            sql.Append($" and ({query.RawQuery})");

        if (sql.Length > 0)
            sql.Remove(0, 4);
        return (sql.ToString(), @paramerters);
    }

    private static void AppendKeyword(string keyword, StringBuilder sql, List<IDataParameter> @paramerters, bool isTrace = true)
    {
        if (string.IsNullOrEmpty(keyword))
            return;

        //status_code
        if (int.TryParse(keyword, out var num) && num != 0 && num - 1000 < 0 && isTrace)
        {
            sql.Append(" and (SpanAttributes['http.status_code']=@HttpStatusCode or SpanAttributes['http.request_content_body'] like @Keyword)");
            paramerters.Add(new ClickHouseParameter() { ParameterName = "HttpStatusCode", Value = num });
            return;
        }


        if (isTrace)
        {
            sql.Append(@" and (SpanAttributes['http.request_content_body'] like @Keyword
                                                    or SpanAttributes['http.url'] like @Keyword
                                                    or SpanAttributes['http.response_content_body'] like @Keyword
                                                    or mapContains(SpanAttributes, 'exception.message') and SpanAttributes['exception.message'] like @Keyword)");
        }
        else
        {
            sql.Append(@" and (Body like @Keyword
                                                    or LogAttributes['HttpTarget'] like @Keyword
                                                    or LogAttributes['http.response_content_body'] like @Keyword
                                                    or mapContains(LogAttributes, 'exception.message') and LogAttributes['exception.message'] like @Keyword)");
        }
        paramerters.Add(new ClickHouseParameter() { ParameterName = "Keyword", Value = $"%{keyword}%" });
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
                item.Value = time.ToLocalTime();
            }
            if (item.Name.StartsWith("resource.", StringComparison.CurrentCultureIgnoreCase))
            {
                var filed = item.Name["resource.".Length..];
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
            else if (item.Name.StartsWith("attributes.", StringComparison.CurrentCultureIgnoreCase))
            {
                var filed = item.Name["attributes.".Length..];
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
        DbType dbType = item.Value is DateTime ? DbType.DateTime2 : DbType.AnsiString;
        switch (item.Type)
        {
            case ConditionTypes.Equal:
                {
                    if (@paramerters.Exists(p => p.ParameterName == paramName))
                        break;
                    sql.Append($" and {fieldName}=@{paramName}");
                    @paramerters.Add(new ClickHouseParameter { ParameterName = paramName, Value = item.Value, DbType = dbType });
                }
                break;
            case ConditionTypes.NotIn:
            case ConditionTypes.In:
                {
                    sql.Append($" and {fieldName} {(item.Type == ConditionTypes.In ? "in" : "not in")} @{paramName}s");
                    @paramerters.Add(new ClickHouseParameter { ParameterName = $"{paramName}s", Value = item.Value, DbType = dbType });
                }
                break;
            case ConditionTypes.LessEqual:
            case ConditionTypes.GreatEqual:
                {
                    sql.Append($" and {fieldName} {(item.Type == ConditionTypes.LessEqual ? "<" : ">")}= @{(item.Type == ConditionTypes.LessEqual ? "lte_" : "gte_")}{paramName}");
                    @paramerters.Add(new ClickHouseParameter { ParameterName = $"{(item.Type == ConditionTypes.LessEqual ? "lte_" : "gte_")}{paramName}", Value = item.Value, DbType = dbType });
                }
                break;
            case ConditionTypes.Less:
            case ConditionTypes.Great:
                {
                    sql.Append($" and {fieldName} {(item.Type == ConditionTypes.LessEqual ? "<" : ">")} @{(item.Type == ConditionTypes.LessEqual ? "lt_" : "gt_")}{paramName}");
                    @paramerters.Add(new ClickHouseParameter { ParameterName = $"{(item.Type == ConditionTypes.LessEqual ? "lt_" : "gt_")}{paramName}", Value = item.Value, DbType = dbType });
                }
                break;
        }
    }

    public static object? ExecuteScalar(this IDbConnection dbConnection, string sql, IDataParameter[]? @parameters = null)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = sql;
        if (@parameters != null && @parameters.Any())
            foreach (var p in @parameters)
                cmd.Parameters.Add(p);
        OpenConnection(dbConnection);
        try
        {
            return cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            ServiceExtensitions.Logger?.LogError(ex, "execute sql error:{rawSql}, paramters:{parameters}", sql, parameters);
            throw;
        }
    }

    private static void OpenConnection(IDbConnection dbConnection)
    {
        switch (dbConnection.State)
        {
            case ConnectionState.Closed:
                dbConnection.Open();
                break;
            case ConnectionState.Broken:
                dbConnection.Close();
                dbConnection.Open();
                break;
        }
    }

    public static List<T> Query<T>(this IDbConnection dbConnection, string sql, IDataParameter[]? @parameters, Func<IDataReader, T> parse)
    {
        using var cmd = dbConnection.CreateCommand();
        cmd.CommandText = sql;
        if (@parameters != null && @parameters.Any())
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
            ServiceExtensitions.Logger?.LogError(ex, "query sql error:{rawSql}, paramters:{parameters}", sql, parameters);
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
        var startTime = Convert.ToDateTime(reader["Timestamp"]);
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
            Timestamp = Convert.ToDateTime(reader["Timestamp"]),
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
        if (name.StartsWith("ResourceAttributes[", StringComparison.CurrentCultureIgnoreCase))
        {
            var filed = requestDto.Name["resource.".Length..];
            appendWhere.Append($" mapContains(ResourceAttributes,'{filed}') and ");
        }
        else if (requestDto.Name.StartsWith("attributes.", StringComparison.CurrentCultureIgnoreCase))
        {
            var filed = requestDto.Name["attributes.".Length..];
            appendWhere.Append($" mapContains({(isLog ? "Log" : "Span")}Attributes,'{filed}') and ");
        }

        AppendAggtype(requestDto, sql, append, name, out var isScalar);

        sql.AppendFormat(" from {0} ", isLog ? MasaStackClickhouseConnection.LogTable : MasaStackClickhouseConnection.TraceTable);
        var (where, @paremeters) = AppendWhere(requestDto, !isLog);
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
                append.Append($" Group By a order by a");
                break;
            case AggregateTypes.DateHistogram:
                sql.Append($"toStartOfInterval({name}, INTERVAL {ConvertInterval(requestDto.Interval)} minute ) as `time`,count() as `count`");
                append.Append($" Group by `time` order by `time`");
                break;
        }
    }

    private static string GetName(string name, bool isLog)
    {
        if (name.Equals("resource.service.name", StringComparison.CurrentCultureIgnoreCase))
        {
            return "ServiceName";
        }
        else if (name.Equals("@timestamp", StringComparison.CurrentCultureIgnoreCase))
        {
            return "Timestamp";
        }
        else if (name.StartsWith("resource.", StringComparison.CurrentCultureIgnoreCase))
        {

            return $"ResourceAttributes['{name[("resource.".Length)..]}']";
        }
        else if (name.StartsWith("attributes.", StringComparison.CurrentCultureIgnoreCase))
        {
            return $"{(isLog ? "Log" : "Span")}Attributes['{name[("attributes.".Length)..]}']";
        }
        else if (!isLog && name.Equals("kind", StringComparison.InvariantCultureIgnoreCase))
        {
            return "SpanKind";
        }
        return name;
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
        var (where, parameters) = AppendWhere(requestDto);
        var text = $"select * from( TraceId from {MasaStackClickhouseConnection.TraceTable} where {where} order by Duration desc) as t limit 1";
        return dbConnection.ExecuteScalar(text, parameters?.ToArray())?.ToString()!;
    }
}
