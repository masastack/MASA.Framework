// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Cliclhouse;

internal class ClickhouseApmService : IApmService
{
    private MasaStackClickhouseConnection _dbConnection;
    private readonly IDbCommand command;
    private readonly ITraceService _traceService;
    private readonly static object lockObj = new();
    private static Dictionary<string, string> serviceOrders = new Dictionary<string, string>() {
        {nameof(ServiceListDto.Name),"ServiceName"},
        {nameof(ServiceListDto.Envs),"env"},
        {nameof(ServiceListDto.Latency),"latency"},
        {nameof(ServiceListDto.Throughput),"throughput"},
        {nameof(ServiceListDto.Failed),"failed"},
    };

    private static Dictionary<string, string> endpointOrders = new Dictionary<string, string>() {
        {nameof(EndpointListDto.Name),"`Attributes.http.target`"},
        {nameof(EndpointListDto.Service),"ServiceName"},
        {nameof(EndpointListDto.Method),"`method`"},
        {nameof(EndpointListDto.Latency),"latency"},
        {nameof(EndpointListDto.Throughput),"throughput"},
        {nameof(EndpointListDto.Failed),"failed"},
    };

    private static Dictionary<string, string> errorOrders = new Dictionary<string, string>() {
        {nameof(ErrorMessageDto.Type),"Type"},
        {nameof(ErrorMessageDto.Message),"Message"},
        {nameof(ErrorMessageDto.LastTime),"`time`"},
        {nameof(ErrorMessageDto.Total),"`total`"}
    };
    const double MILLSECOND = 1e6;

    public ClickhouseApmService(MasaStackClickhouseConnection dbConnection, ITraceService traceService)
    {
        _traceService = traceService;
        _dbConnection = dbConnection;
        command = dbConnection.CreateCommand();
        if (_dbConnection.State == ConnectionState.Closed)
            _dbConnection.Open();
    }

    public Task<PaginatedListBase<ServiceListDto>> ServicePageAsync(BaseApmRequestDto query)
    {
        query.IsServer = true;
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by ServiceName";
        var countSql = $"select count(1) from(select count(1) from {Constants.TraceTableFull} where {where} {groupby})";
        PaginatedListBase<ServiceListDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query, serviceOrders, defaultSort: "ServiceName");
        var sql = $@"select * from(
select
ServiceName,
arrayStringConcat(groupUniqArray(`Resource.service.namespace`)) env,
floor(AVG(Duration/{MILLSECOND})) latency,
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
        var orderBy = GetOrderBy(query, new());
        var sql = $@"select * from( select ResourceAttributesValues[indexOf(ResourceAttributesKeys,'service.instance.id')] instance`,
AVG(Duration/{MILLSECOND}) Latency,
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
        var orderBy = GetOrderBy(query, new());
        var sql = $@"select * from( select `Attributes.http.target`,ServiceName,SpanAttributesValues[indexOf(SpanAttributesKeys,'http.method')] `method`,
AVG(Duration{MILLSECOND}) Latency,
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
        query.IsServer = true;
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by ServiceName,`Attributes.http.target`,SpanAttributesValues[indexOf(SpanAttributesKeys,'http.method')]";
        var countSql = $"select count(1) from(select count(1) from {Constants.TraceTableFull} where {where} {groupby})";
        PaginatedListBase<EndpointListDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query, endpointOrders);
        var sql = $@"select * from( select `Attributes.http.target`,ServiceName,SpanAttributesValues[indexOf(SpanAttributesKeys,'http.method')] `method`,
floor(AVG(Duration/{MILLSECOND})) latency,
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
        query.IsServer = true;
        var (where, parameters) = AppendWhere(query);
        var result = new List<ChartLineDto>();
        var groupby = "group by ServiceName ,`time` order by ServiceName ,`time`";
        var sql = $@"select 
ServiceName,
toStartOfInterval(`Timestamp` , INTERVAL {GetPeriod(query)} ) as `time`,
floor(avg(Duration/{MILLSECOND})) `latency`,
floor(quantile(0.95)(Duration/{MILLSECOND})) `p95`,
floor(quantile(0.99)(Duration/{MILLSECOND})) `p99`,
round(sum(has(['{string.Join("','", query.GetErrorStatusCodes())}'],`Attributes.http.status_code`))*100.0/count(1),2) `failed`,
round(count(1)*1.0/DATEDIFF(MINUTE ,toDateTime(@startTime),toDateTime (@endTime)),2) `throughput`
from {Constants.TraceTableFull} where {where} {groupby}";
        lock (lockObj)
        {
            using var reader = Query(sql, parameters);
            SetChartData(result, reader);
        }
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

        lock (lockObj)
        {
            using var readerPrevious = Query(sql, parameters);
            SetChartData(result, readerPrevious, isPrevious: true);
        }
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
        var p95 = Convert.ToDouble(Scalar($"select floor(quantile(0.95)(Duration/{MILLSECOND})) p95 from {Constants.TraceTableFull} where {where}", parameters));
        if (p95 is not double.NaN)
            result.P95 = (long)Math.Floor(p95);
        var sql = $@"select Duration/{MILLSECOND},count(1) total from {Constants.TraceTableFull} where {where} group by Duration order by Duration";
        var list = new List<ChartPointDto>();
        lock (lockObj)
        {
            using var reader = Query(sql, parameters);
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
        }
        result.Latencies = list;
        return Task.FromResult(result);
    }

    public Task<PaginatedListBase<ErrorMessageDto>> ErrorMessagePageAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        var (where, parameters) = AppendWhere(query);
        var groupby = $"group by Type,Message{(string.IsNullOrEmpty(query.Endpoint) ? "" : ",Endpoint")}";
        var countSql = $"select count(1) from (select Attributes.exception.type as Type,Attributes.exception.message as Message,max(Timestamp) time,count(1) from {Constants.ErrorTableFull} where {where} {groupby})";
        PaginatedListBase<ErrorMessageDto> result = new() { Total = Convert.ToInt64(Scalar(countSql, parameters)) };
        var orderBy = GetOrderBy(query, errorOrders);
        var sql = $@"select * from( select Attributes.exception.type as Type,Attributes.exception.message as Message,max(Timestamp) time,count(1) total from {Constants.ErrorTableFull} where {where} {groupby} {orderBy} @limit)";
        SetData(sql, parameters, result, query, reader => new ErrorMessageDto()
        {
            Type = reader[0]?.ToString()!,
            Message = reader[1]?.ToString()!,
            LastTime = Convert.ToDateTime(reader[2])!,
            Total = Convert.ToInt32(reader[3]),
        });
        return Task.FromResult(result);
    }

    private void SetData<TResult>(string sql, List<IDbDataParameter> parameters, PaginatedListBase<TResult> result, BaseApmRequestDto query, Func<IDataReader, TResult> parseFn) where TResult : class
    {
        var start = (query.Page - 1) * query.PageSize;
        if (result.Total - start > 0)
        {
            lock (lockObj)
            {
                using var reader = Query(sql.Replace("@limit", $"limit {start},{query.PageSize}"), parameters);
                result.Result = new();
                while (reader.NextResult())
                    while (reader.Read())
                        result.Result.Add(parseFn(reader));
            }
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
            if (query.IsLog.HasValue && query.IsLog.Value)
            {
                sql.AppendLine(" and indexOf(LogAttributesKeys,'RequestPath')>=0 and LogAttributesValues[indexOf(LogAttributesKeys,'RequestPath')] LIKE @endpoint");
                parameters.Add(new ClickHouseParameter { ParameterName = "endpoint", Value = $"{traceQuery.Endpoint}%" });
            }
            else
            {
                sql.AppendLine(" and Attributes.http.target=@endpoint");
                parameters.Add(new ClickHouseParameter { ParameterName = "endpoint", Value = traceQuery.Endpoint });
            }
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

        if (!string.IsNullOrEmpty(query.Queries) && query.Queries.Trim().Length > 0)
        {
            if (!query.Queries.Trim().StartsWith("and ", StringComparison.CurrentCultureIgnoreCase))
                sql.Append(" and ");
            sql.AppendLine(query.Queries);
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
        command.CommandText = sql;
        SetParameters(parameters);
        return command.ExecuteReader();
    }

    private object Scalar(string sql, IEnumerable<IDbDataParameter> parameters)
    {
        lock (lockObj)
        {
            command.CommandText = sql;
            SetParameters(parameters);
            return command.ExecuteScalar()!;
        }
    }

    private void SetParameters(IEnumerable<IDbDataParameter> parameters)
    {
        if (command.Parameters.Count > 0)
            command.Parameters.Clear();
        if (parameters != null && parameters.Any())
            foreach (var param in parameters)
                command.Parameters.Add(param);
    }

    private static string? GetOrderBy(BaseApmRequestDto query, Dictionary<string, string> sortFields, string? defaultSort = null, bool isDesc = false)
    {
        if (!string.IsNullOrEmpty(query.OrderField) && sortFields.ContainsKey(query.OrderField))
        {
            if (!query.IsDesc.HasValue)
                return $"order by {sortFields[query.OrderField]}";
            return $"order by {sortFields[query.OrderField]}{(query.IsDesc.Value ? "" : " desc")}";
        }

        if (string.IsNullOrEmpty(defaultSort))
            return null;
        return $"order by {defaultSort}{(isDesc ? " desc" : "")}";
    }

    public void Dispose()
    {
        _dbConnection.Close();
        _dbConnection.Dispose();
    }

    public Task<IEnumerable<ChartLineCountDto>> GetErrorChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsLog = true;
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {Constants.LogTableFull} where {where} and SeverityText='Error' {groupby}";

        return Task.FromResult(getChartCountData(sql, parameters, query.ComparisonType));
    }

    private IEnumerable<ChartLineCountDto> getChartCountData(string sql, IEnumerable<IDbDataParameter> parameters, ComparisonTypes? comparisonTypes = null)
    {
        var result = new List<ChartLineCountDto>();
        lock (lockObj)
        {
            using var currentReader = Query(sql, parameters);
            SetChartCountData(result, currentReader);
        }

        if (comparisonTypes.HasValue && (comparisonTypes.Value == ComparisonTypes.DayBefore || comparisonTypes.Value == ComparisonTypes.WeekBefore))
        {
            var day = comparisonTypes.Value == ComparisonTypes.DayBefore ? -1 : -7;
            var paramStartTime = parameters.First(p => p.ParameterName == "startTime");
            paramStartTime.Value = ((DateTime)paramStartTime.Value!).AddDays(day);

            var paramEndTime = parameters.First(p => p.ParameterName == "endTime");
            paramEndTime.Value = ((DateTime)paramEndTime.Value!).AddDays(day);

            lock (lockObj)
            {
                using var previousReader = Query(sql, parameters);
                SetChartCountData(result, previousReader, true);
            }
        }

        return result;
    }

    private void SetChartCountData(List<ChartLineCountDto> result, IDataReader reader, bool isPrevious = false)
    {
        if (!reader.NextResult())
            return;
        ChartLineCountDto? current = null;
        while (reader.Read())
        {
            var name = reader[0].ToString()!;
            var time = new DateTimeOffset(Convert.ToDateTime(reader[0])).ToUnixTimeSeconds();
            if (current == null || current.Name != name)
            {
                if (isPrevious && result.Exists(item => item.Name == name))
                {
                    current = result.First(item => item.Name == name);
                }
                else
                {
                    current = new ChartLineCountDto
                    {
                        Name = name,
                        Previous = new List<ChartLineCountItemDto>(),
                        Currents = new List<ChartLineCountItemDto>()
                    };
                    result.Add(current);
                }
            }

            ((List<ChartLineCountItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
                new()
                {
                    Value = reader[1],
                    Time = time
                });
        }
    }

    public Task<IEnumerable<ChartLineCountDto>> GetEndpointChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = false;
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {Constants.TraceTable} where {where} {groupby}";

        return Task.FromResult(getChartCountData(sql, parameters, query.ComparisonType));
    }

    public Task<IEnumerable<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsLog = true;
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by `time` order by `time`";
        var sql = $@"select 
toStartOfInterval(`Timestamp` , INTERVAL  {GetPeriod(query)} ) as `time`,
count(1) `total`
from {Constants.LogTableFull} where {where} {groupby}";
        return Task.FromResult(getChartCountData(sql, parameters, query.ComparisonType));
    }

    private string GetPeriod(BaseApmRequestDto query)
    {
        var reg = new Regex(@"/d+");
        if (string.IsNullOrEmpty(query.Period) || !reg.IsMatch(query.Period))
        {
            return GetDefaultPeriod(query.End - query.Start);
        }
        var unit = reg.Replace(query.Period, "").Trim().ToLower();
        var units = new string[] { "year", "month", "week", "day", "hour", "minute", "second" };
        var find = units.FirstOrDefault(s => s.StartsWith(unit));
        if (string.IsNullOrEmpty(find))
            find = "minute";
        return $"{reg.Match(query.Period).Result} {unit}";
    }

    private string GetDefaultPeriod(TimeSpan timeSpan)
    {
        if ((int)timeSpan.TotalMinutes < 1)
        {
            return "5 second";
        }

        if ((int)timeSpan.TotalHours < 1)
        {
            return "1 minute";
        }

        var days = (int)timeSpan.TotalDays;
        if (days <= 0)
        {
            if ((int)timeSpan.TotalHours - 12 <= 0)
            {
                return "1 minute";
            }
            return "30 minute";
        }

        if (days - 7 <= 0)
        {
            return "1 hour";
        }

        if (days - 30 <= 0)
        {
            return "1 day";
        }

        if (days - 365 <= 0)
        {
            return "1 week";
        }

        return "1 month";
    }

    public Task<IEnumerable<ChartPointDto>> GetTraceErrorsAsync(ApmEndpointRequestDto query)
    {
        query.IsServer = default;
        query.IsLog = true;
        var (where, parameters) = AppendWhere(query);
        var groupby = "group by `SpanId` order by `SpanId`";
        var sql = $@"select 
SpanId,
count(1) `total`
from {Constants.ErrorTableFull} where {where} {groupby}";
        var list = new List<ChartPointDto>();
        lock (lockObj)
        {
            using var reader = Query(sql, parameters);
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
        }
        return Task.FromResult(list.AsEnumerable());
    }    
}
