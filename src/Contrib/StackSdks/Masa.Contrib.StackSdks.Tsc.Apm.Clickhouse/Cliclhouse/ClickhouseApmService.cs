// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Cliclhouse;

internal class ClickhouseApmService : IApmService
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbCommand _command;
    private readonly ITraceService _traceService;

    public ClickhouseApmService(IDbConnection dbConnection, ITraceService traceService)
    {
        _traceService = traceService;
        _dbConnection = dbConnection;
        if (_dbConnection.State == ConnectionState.Closed)
            _dbConnection.Open();
        _command = dbConnection.CreateCommand();
    }

    public Task<PaginatedListBase<ServiceListDto>> ServicePageAsync(BaseApmRequestDto query)
    {
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by ServiceName";
        var countSql = $"select count(1) from(select count(1) from {Constants.TraceTableFull} where {where} {groupby})";
        PaginatedListBase<ServiceListDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query, "ServiceName");
        var sql = $@"select * from(
select
ServiceName,
arrayStringConcat(groupUniqArray(`Resource.service.namespace`)) env,
floor(AVG(Duration)) latency,
round(count(1)*1.0/DATEDIFF(MINUTE ,toDateTime(@startTime),toDateTime (@endTime)),2) throughput,
round(sum(has(['{string.Join("','", query.GetErrorStatusCodes())}'],`Attributes.http.status_code`))*100.0/count(1),2) failed
from {Constants.TraceTableFull} where {where} {groupby} {orderBy} @limit)";
        SetData(sql, parameters, result, query, reader => new ServiceListDto()
        {
            Name = reader[0].ToString()!,
            Envs = reader[1]?.ToString()?.Split(',') ?? Array.Empty<string>(),
            Latency = (long)Math.Floor(Convert.ToDouble(reader[2])),
            Throughput = Math.Round(Convert.ToDouble(reader[3]), 2),
            Failed = Math.Round(Convert.ToDouble(reader[4]), 2),
        });
        return Task.FromResult(result);
    }

    public Task<PaginatedListBase<EndpointListDto>> InstancePageAsync(BaseApmRequestDto query)
    {
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by instance";
        var countSql = $"select count(1) from(select count(1) from {Constants.TraceTableFull} where {where} {groupby})";
        PaginatedListBase<EndpointListDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query);
        var sql = $@"select * from( select ResourceAttributesValues[indexOf(ResourceAttributesKeys,'service.instance.id')] instance`,
AVG(Duration) Latency,
count(1)*1.0/DATEDIFF(MINUTE ,toDateTime(@startTime),toDateTime (@endTime)) throughput
sum(has(['{string.Join("','", query.GetErrorStatusCodes())}'],`Attributes.http.status_code`))/count(1) failed
from {where} {groupby} {orderBy} @limit)";
        SetData(sql, parameters, result, query, reader => new EndpointListDto()
        {
            Name = reader[0].ToString()!,
            Latency = (long)Math.Floor(Convert.ToDouble(reader[1])),
            Throughput = Math.Round(Convert.ToDouble(reader[2]), 2),
            Failed = Math.Round(Convert.ToDouble(reader[3]), 2)
        });
        return Task.FromResult(result);
    }

    public Task<PaginatedListBase<EndpointListDto>> DependencyPageAsync(BaseApmRequestDto query)
    {
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by ServiceName,`Attributes.http.target`,`method`";
        var countSql = $"select count(1) from(select count(1) from {Constants.TraceTableFull} where {where} {groupby})";
        PaginatedListBase<EndpointListDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query);
        var sql = $@"select * from( select `Attributes.http.target`,ServiceName,SpanAttributesValues[indexOf(SpanAttributesKeys,'http.method')] `method`,
AVG(Duration) Latency,
count(1)*1.0/DATEDIFF(MINUTE ,toDateTime(@startTime),toDateTime (@endTime)) throughput
sum(has(['{string.Join(",'", query.GetErrorStatusCodes())}'],`Attributes.http.status_code`))/count(1) failed
from {where} {groupby} {orderBy} @limit)";
        SetData(sql, parameters, result, query, reader => new EndpointListDto()
        {
            Name = reader[0].ToString()!,
            Service = reader[1]?.ToString()!,
            Method = reader[2]?.ToString()!,
            Latency = (long)Math.Floor(Convert.ToDouble(reader[3])),
            Throughput = Math.Round(Convert.ToDouble(reader[4]), 2),
            Failed = Math.Round(Convert.ToDouble(reader[5]), 2)
        });
        return Task.FromResult(result);
    }

    public Task<PaginatedListBase<EndpointListDto>> EndpointPageAsync(BaseApmRequestDto query)
    {
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by ServiceName,`Attributes.http.target`,SpanAttributesValues[indexOf(SpanAttributesKeys,'http.method')]";
        var countSql = $"select count(1) from(select count(1) from {Constants.TraceTableFull} where {where} {groupby})";
        PaginatedListBase<EndpointListDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query);
        var sql = $@"select * from( select `Attributes.http.target`,ServiceName,SpanAttributesValues[indexOf(SpanAttributesKeys,'http.method')] `method`,
floor(AVG(Duration)) Latency,
round(count(1)*1.0/DATEDIFF(MINUTE ,toDateTime(@startTime),toDateTime (@endTime)),2) throughput,
round(sum(has(['{string.Join("','", query.GetErrorStatusCodes())}'],`Attributes.http.status_code`))*100.0/count(1),2) failed
from {Constants.TraceTableFull} where {where} and Attributes.http.target!='' {groupby} {orderBy} @limit)";
        SetData(sql, parameters, result, query, reader => new EndpointListDto()
        {
            Name = reader[0].ToString()!,
            Service = reader[1]?.ToString()!,
            Method = reader[2]?.ToString()!,
            Latency = (long)Math.Floor(Convert.ToDouble(reader[3])),
            Throughput = Math.Round(Convert.ToDouble(reader[4]), 2),
            Failed = Math.Round(Convert.ToDouble(reader[5]), 2)
        });
        return Task.FromResult(result);
    }

    public Task<IEnumerable<ChartLineDto>> ChartDataAsync(BaseApmRequestDto query)
    {
        var (where, parameters) = AppendWhere(query);
        var result = new List<ChartLineDto>();
        var groupby = "group by ServiceName ,`time` order by ServiceName ,`time`";
        var sql = $@"select 
ServiceName,
toStartOfInterval(`Timestamp` , INTERVAL 1 minute ) as `time`,
floor(avg(Duration)) latency,
floor(quantile(0.95)(Duration)) p95,
floor(quantile(0.99)(Duration)) p99,
round(sum(has(['{string.Join("','", query.GetErrorStatusCodes())}'],`Attributes.http.status_code`))*100.0/count(1),2) failed,
round(count(1)*1.0/DATEDIFF(MINUTE ,toDateTime(@startTime),toDateTime (@endTime)),2) throughput
from {Constants.TraceTableFull} where {where} {groupby}";
        using var reader = Query(sql, parameters);
        SetChartData(result, reader);
        GetPreviousChartData(query, sql, parameters, result);
        return Task.FromResult(result.AsEnumerable());
    }

    private void GetPreviousChartData(BaseApmRequestDto query, string sql, List<IDbDataParameter> parameters, List<ChartLineDto> result)
    {
        if (!query.ComparisonType.HasValue)
            return;

        int day = 0;
        switch (query.ComparisonType.Value)
        {
            case ComparisonTypes.DayBefore:
                day = -1;
                break;
            case ComparisonTypes.WeekBefore:
                day = -7;
                break;
        }
        if (day == 0)
            return;

        var paramStartTime = parameters.First(p => p.ParameterName == "startTime");
        paramStartTime.Value = ((DateTime)paramStartTime.Value!).AddDays(day);

        var paramEndTime = parameters.First(p => p.ParameterName == "endTime");
        paramEndTime.Value = ((DateTime)paramEndTime.Value!).AddDays(day);

        using var readerPrevious = Query(sql, parameters);
        SetChartData(result, readerPrevious, isPrevious: true);
    }

    private static void SetChartData(List<ChartLineDto> result, IDataReader reader, bool isPrevious = false)
    {
        if (!reader.NextResult())
            return;
        ChartLineDto? current = null;
        while (reader.Read())
        {
            var name = reader[0].ToString()!;
            var time = new DateTimeOffset(Convert.ToDateTime(reader[1])).ToUnixTimeSeconds();
            if (current == null || current.Name != name)
            {
                if (isPrevious && result.Exists(item => item.Name == name))
                {
                    current = result.First(item => item.Name == name);
                }
                else
                {
                    current = new ChartLineDto
                    {
                        Name = name,
                        Previous = new List<ChartLineItemDto>(),
                        Currents = new List<ChartLineItemDto>()
                    };
                    result.Add(current);
                }
            }

            ((List<ChartLineItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
                new()
                {
                    Latency = (long)Math.Floor(Convert.ToDouble(reader[2])),
                    P95 = Math.Round(Convert.ToDouble(reader[3]), 2, MidpointRounding.ToZero),
                    P99 = Math.Round(Convert.ToDouble(reader[4]), 2, MidpointRounding.ToZero),
                    Failed = Math.Round(Convert.ToDouble(reader[5]), 2, MidpointRounding.ToZero),
                    Throughput = Math.Round(Convert.ToDouble(reader[6]), 2, MidpointRounding.ToZero),
                    Time = time
                });
        }
    }

    public Task<EndpointLatencyDistributionDto> EndpointLatencyDistributionAsync(ApmEndpointRequestDto query)
    {
        var (where, parameters) = AppendWhere(query);
        var result = new EndpointLatencyDistributionDto();
        var p95 = Convert.ToDouble(Scalar($"select floor(quantile(0.95)(Duration)) p95 from {Constants.TraceTableFull} where {where}", parameters));
        if (p95 is not double.NaN)
            result.P95 = (long)Math.Floor(p95);
        var sql = $@"select Duration,count(1) total from {Constants.TraceTableFull} where {where} group by Duration order by Duration";
        using var reader = Query(sql, parameters);
        var list = new List<ChartPointDto>();
        while (reader.NextResult())
            while (reader.Read())
            {
                var item = new ChartPointDto()
                {
                    X = reader[0].ToString()!,
                    Y = reader[1]?.ToString()!
                };
                list.Add(item);
            }
        result.Latencies = list;
        return Task.FromResult(result);
    }

    public Task<PaginatedListBase<ErrorMessageDto>> ErrorMessagePageAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        var (where, parameters) = AppendWhere(query);
        var groupby = $"group by Type{(string.IsNullOrEmpty(query.Endpoint) ? "" : ",Endpoint")}";
        var countSql = $"select count(1) from (select Attributes.exception.type as Type,max(Timestamp) time,count(1) from {Constants.ErrorTableFull} where {where} {groupby})";
        PaginatedListBase<ErrorMessageDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query);
        var sql = $@"select * from( select Attributes.exception.type as Type,max(Timestamp) time,count(1) total from {Constants.ErrorTableFull} where {where} {groupby} {orderBy} @limit)";
        SetData(sql, parameters, result, query, reader => new ErrorMessageDto()
        {
            Type = reader[0]?.ToString()!,
            LastTime = Convert.ToDateTime(reader[1])!,
            Total = Convert.ToInt32(reader[2]),
        });
        return Task.FromResult(result);
    }

    private void SetData<TResult>(string sql, List<IDbDataParameter> parameters, PaginatedListBase<TResult> result, BaseApmRequestDto query, Func<IDataReader, TResult> parseFn) where TResult : class
    {
        var start = (query.Page - 1) * query.PageSize;
        if (result.Total - start > 0)
        {
            using var reader = Query(sql.Replace("@limit", $"limit {start},{query.PageSize}"), parameters);
            result.Result = new();
            while (reader.NextResult())
                while (reader.Read())
                    result.Result.Add(parseFn(reader));
        }
    }

    private static (string where, List<IDbDataParameter> parameters) AppendWhere<TQuery>(TQuery query) where TQuery : BaseApmRequestDto
    {
        List<IDbDataParameter> parameters = new();
        var sql = new StringBuilder();
        sql.AppendLine(" Timestamp between @startTime and @endTime");
        parameters.Add(new ClickHouseParameter { ParameterName = "startTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.Start), DbType = DbType.DateTime });
        parameters.Add(new ClickHouseParameter { ParameterName = "endTime", Value = MasaStackClickhouseConnection.ToTimeZone(query.End), DbType = DbType.DateTime });
        if (!string.IsNullOrEmpty(query.Env))
        {
            sql.AppendLine(" and Resource.service.namespace=@environment");
            parameters.Add(new ClickHouseParameter { ParameterName = "environment", Value = query.Env });
        }
        if (!string.IsNullOrEmpty(query.Service))
        {
            sql.AppendLine(" and ServiceName=@serviceName");
            parameters.Add(new ClickHouseParameter { ParameterName = "serviceName", Value = query.Service });
        }
        if (query.IsServer.HasValue)
        {
            sql.AppendLine(" and SpanKind=@spanKind");
            parameters.Add(new ClickHouseParameter { ParameterName = "spanKind", Value = query.IsServer.Value ? "SPAN_KIND_SERVER" : "SPAN_KIND_CLIENT" });
        }

        if (query is ApmEndpointRequestDto traceQuery && !string.IsNullOrEmpty(traceQuery.Endpoint))
        {
            sql.AppendLine(" and Attributes.http.target=@endpoint");
            parameters.Add(new ClickHouseParameter { ParameterName = "endpoint", Value = traceQuery.Endpoint });
        }

        if (query is ApmTraceLatencyRequestDto durationQuery)
        {
            if (durationQuery.LatMin > 0 && durationQuery.LatMax > 0)
            {
                sql.AppendLine(" and Duration between @minDuration and @maxDuration");
                parameters.Add(new ClickHouseParameter { ParameterName = "minDuration", Value = durationQuery.LatMin });
                parameters.Add(new ClickHouseParameter { ParameterName = "maxDuration", Value = durationQuery.LatMax });
            }
            else if (durationQuery.LatMin > 0)
            {
                sql.AppendLine(" and Duration >=@minDuration");
                parameters.Add(new ClickHouseParameter { ParameterName = "minDuration", Value = durationQuery.LatMin });
            }
            else if (durationQuery.LatMax > 0)
            {
                sql.AppendLine(" and Duration <=@maxDuration");
                parameters.Add(new ClickHouseParameter { ParameterName = "maxDuration", Value = durationQuery.LatMax });
            }
        }

        return (sql.ToString(), parameters);
    }

    public async Task<PaginatedListBase<TraceResponseDto>> TraceLatencyDetailAsync(ApmTraceLatencyRequestDto query)
    {
        var queryDto = new BaseRequestDto
        {
            Start = query.Start,
            End = query.End,
            Endpoint = query.Endpoint,
            Service = query.Service!
        };
        var conditions = new List<FieldConditionDto>();
        if (!string.IsNullOrEmpty(query.Env))
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "Resource.service.namespace",
                Type = ConditionTypes.Equal,
                Value = query.Env
            });
        }
        if (query.LatMin.HasValue && query.LatMin.Value >= 0)
        {
            conditions.Add(new FieldConditionDto
            {
                Name = "Duration",
                Type = ConditionTypes.GreatEqual,
                Value = query.LatMin.Value,
            });
        }

        if (query.LatMax.HasValue && query.LatMax.Value >= 0 && (
            !query.LatMin.HasValue
            || query.LatMin.HasValue && query.LatMax - query.LatMin.Value > 0))
            conditions.Add(new FieldConditionDto
            {
                Name = "Duration",
                Type = ConditionTypes.LessEqual,
                Value = query.LatMax.Value,
            });
        if (conditions.Any())
            queryDto.Conditions = conditions;

        return await _traceService.ListAsync(queryDto);
    }

    private IDataReader Query(string sql, IEnumerable<IDbDataParameter> parameters)
    {
        _command.CommandText = sql;
        SetParameters(parameters);
        return _command.ExecuteReader();
    }

    private object Scalar(string sql, IEnumerable<IDbDataParameter> parameters)
    {
        _command.CommandText = sql;
        SetParameters(parameters);
        return _command.ExecuteScalar()!;
    }

    private void SetParameters(IEnumerable<IDbDataParameter> parameters)
    {
        if (_command.Parameters.Count > 0)
            _command.Parameters.Clear();
        if (parameters != null && parameters.Any())
            foreach (var param in parameters)
                _command.Parameters.Add(param);
    }

    private static string? GetOrderBy(BaseApmRequestDto query, string? defaultSort = null, bool isDesc = false)
    {
        if (string.IsNullOrEmpty(query.OrderField))
        {
            if (string.IsNullOrEmpty(defaultSort))
                return null;
            return $"order by {defaultSort}{(isDesc ? " desc" : "")}";
        }
        if (!query.IsDesc.HasValue)
            return $"order by {query.OrderField}";
        return $"order by {query.OrderField}{(query.IsDesc.Value ? "" : " desc")}";
    }

    public void Dispose()
    {
        _dbConnection.Close();
        _dbConnection.Dispose();
    }
}
