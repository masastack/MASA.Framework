// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest;

internal static class IElasticClientExtenstion
{
    public static async Task SearchAsync<TResult, TQuery>(this IElasticClient client,
        string indexName,
        TQuery query,
        Func<QueryContainerDescriptor<TResult>, TQuery, QueryContainer>? condition = null,
        Action<ISearchResponse<TResult>, TQuery>? result = null,
        Func<AggregationContainerDescriptor<TResult>, TQuery, IAggregationContainer>? aggregate = null,
        Func<ValueTuple<bool, int, int>>? page = null,
        Func<SortDescriptor<TResult>, TQuery, SortDescriptor<TResult>>? sort = null,
        Func<string[]>? includeFields = null,
        Func<string[]>? excludeFields = null
        ) where TResult : class where TQuery : class
    {
        try
        {
            SearchDescriptor<TResult> func(SearchDescriptor<TResult> searchDescriptor)
            {
                if (condition is not null)
                    searchDescriptor = searchDescriptor.Query(queryContainer => condition.Invoke(queryContainer, query));
                int curPage = 0, pageSize = 0;
                bool hasPage = false;
                if (page != null)
                {
                    var pageData = page.Invoke();
                    hasPage = pageData.Item1;
                    curPage = pageData.Item2;
                    pageSize = pageData.Item3;
                }
                searchDescriptor = SetPageSize(searchDescriptor, hasPage, curPage, pageSize);
                if (sort != null)
                    searchDescriptor = searchDescriptor.Sort(sortDescriptor => sort(sortDescriptor, query));
                if (includeFields != null || excludeFields != null)
                {
                    searchDescriptor = searchDescriptor.Source(source =>
                    {
                        if (includeFields != null)
                            source = source.Includes(f => f.Fields(includeFields.Invoke()));
                        if (excludeFields != null)
                            source = source.Excludes(f => f.Fields(excludeFields.Invoke()));
                        return source;
                    });
                }

                if (aggregate != null)
                {
                    searchDescriptor = searchDescriptor.Aggregations(agg => aggregate?.Invoke(agg, query));
                }
                return searchDescriptor;
            }
            var searchResponse = await client.SearchAsync<TResult>(s => func(s.Index(indexName)));
            searchResponse.FriendlyElasticException();
            if (searchResponse.IsValid)
            {
                result?.Invoke(searchResponse, query);
            }
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"SearchAsync execute error {ex.Message}");
        }
    }

    private static SearchDescriptor<T> SetPageSize<T>(SearchDescriptor<T> container, bool hasPage, int page, int size) where T : class
    {
        if (!hasPage)
            return container.Size(size);

        var start = (page - 1) * size;

        if (ElasticConst.MaxRecordCount - start - size <= 0)
            throw new UserFriendlyException($"elastic query data max count must be less {ElasticConst.MaxRecordCount}, please input more condition to limit");

        return container.Size(size).From(start);
    }

    #region mapping
    /// <summary>
    /// 获取mapping
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="indexName"></param>                    
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<MappingResponseDto>> GetMappingAsync(this ICaller caller, string indexName, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(indexName);
        var path = $"/{indexName}/_mapping";
        var result = await caller.GetAsync<JsonElement>(path, false, token);

        if (!result.TryGetProperty(indexName, out JsonElement root) ||
            !root.TryGetProperty("mappings", out JsonElement mapping))
        {
            return default!;
        }

        return GetRepProperties(mapping, default!)!;
    }

    private static IEnumerable<MappingResponseDto>? GetRepProperties(JsonElement node, string? parentName = default)
    {
        if (node.ValueKind != JsonValueKind.Object)
            return default;

        var properties = GetProperties(node);
        if (properties == null || properties.Value.ValueKind != JsonValueKind.Object)
            return default;

        var result = new List<MappingResponseDto>();
        var obj = properties.Value.EnumerateObject();
        foreach (var item in obj)
        {
            var type = GetType(item.Value);
            var name = $"{parentName}{item.Name}";
            if (string.IsNullOrEmpty(type))
            {
                var children = GetRepProperties(item.Value, $"{name}.");
                if (children != null && children.Any())
                    result.AddRange(children);
            }
            else
            {
                var model = new ElasticseacherMappingResponseDto
                {
                    Name = name,
                    Type = type
                };
                SetKeyword(item.Value, model);
                result.Add(model);
            }
        }
        return result;
    }

    private static JsonElement? GetProperties(JsonElement value)
    {
        if (value.TryGetProperty(MappingConst.PROPERTY, out JsonElement result))
            return result;
        return null;
    }

    private static string? GetType(JsonElement value)
    {
        if (value.TryGetProperty(MappingConst.TYPE, out JsonElement element))
            return element.ToString();
        return default;
    }

    private static void SetKeyword(JsonElement value, ElasticseacherMappingResponseDto model)
    {
        if (value.TryGetProperty(MappingConst.FIELD, out JsonElement fields) &&
       fields.TryGetProperty(MappingConst.KEYWORD, out JsonElement element))
        {
            if (element.TryGetProperty(MappingConst.TYPE, out JsonElement type) && type.ToString() == MappingConst.KEYWORD)
                model.IsKeyword = true;
            if (element.TryGetProperty(MappingConst.MAXLENGTH, out JsonElement maxLength))
                model.MaxLenth = maxLength.GetInt32();
        }
    }

    public static void FriendlyElasticException<T>(this ISearchResponse<T> response, string? message = null) where T : class
    {
        if (!response.IsValid)
        {
            throw new UserFriendlyException($"elastic query error: status:{response.ServerError?.Status},message:{response.OriginalException?.Message ?? response.ServerError?.ToString()}");
        }
    }
    #endregion

    #region log
    public static async Task<PaginationDto<LogResponseDto>> SearchLogAsync(this IElasticClient client, LogRequestDto query)
    {
        PaginationDto<LogResponseDto> result = default!;
        await client.SearchAsync<object, LogRequestDto>(ElasticConst.Log.IndexName,
        query: query,
        condition: SearchFn,
        result: (response, q) =>
        {
            result = SetLogResult(response);
        },
        sort: SortFn,
        page: () => ValueTuple.Create(true, query.Page, query.Size));
        return result;
    }

    public static async Task<object> AggregateLogAsync(this IElasticClient client, SimpleAggregateRequestDto query)
    {
        object result = default!;
        await client.SearchAsync<object, SimpleAggregateRequestDto>(ElasticConst.Log.IndexName,
        query,
        condition: SearchFn,
        result: (response, q) =>
        {
            result = SetAggregationResult(response, q);
        },
        aggregate: (agg, query) => AggregationFn(agg, query, true),
        sort: SortFn,
        page: () => ValueTuple.Create(true, query.Page, query.Size));
        return result;
    }
    #endregion

    #region trace
    public static async Task<PaginationDto<TraceResponseDto>> SearchTraceAsync(this IElasticClient client, TraceRequestDto query)
    {
        PaginationDto<TraceResponseDto> result = default!;
        await SearchAsync<object, TraceRequestDto>(client, ElasticConst.Trace.IndexName,
        query: query,
        condition: SearchFn,
        result: (response, q) =>
        {
            result = SetTraceResult(response);
        },
        sort: SortFn,
        page: () => ValueTuple.Create(true, query.Page, query.Size));
        return result;
    }

    public static async Task<object> AggregateTraceAsync(this IElasticClient client, SimpleAggregateRequestDto query)
    {
        object result = default!;
        await SearchAsync<object, SimpleAggregateRequestDto>(client, ElasticConst.Trace.IndexName,
        query,
        condition: SearchFn,
        result: (response, q) =>
        {
            result = SetAggregationResult(response, q);
        },
        (agg, query) => AggregationFn(agg, query, false),
        sort: SortFn,
        page: () => ValueTuple.Create(true, query.Page, query.Size));
        return result;
    }
    #endregion

    private static QueryContainer SearchFn<TQuery, TResult>(QueryContainerDescriptor<TResult> queryContainer, TQuery query) where TQuery : BaseRequestDto where TResult : class
    {
        var list = new List<Func<QueryContainerDescriptor<TResult>, QueryContainer>>();
        bool isLog = false;
        if (query is LogRequestDto)
            isLog = true;

        if (!string.IsNullOrEmpty(query.RawQuery))
        {
            list.Add(queryContainer => queryContainer.Raw(query.RawQuery));
        }
        if (query.Start > DateTime.MinValue && query.End > DateTime.MinValue && query.Start < query.End)
        {
            list.Add(queryContainer => queryContainer.DateRange(dateRangeQuery => dateRangeQuery.GreaterThanOrEquals(query.Start).LessThanOrEquals(query.End).Field(isLog ? ElasticConst.Log.Timestamp : ElasticConst.Trace.Timestamp)));
        }
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            list.Add(queryContainer => queryContainer.QueryString(queryString => queryString.Query(query.Keyword)));
        }

        query.AppendConditions();
        var conditions = AddFilter(query);
        if (conditions != null && conditions.Any())
        {
            var mappings = isLog ? ElasticConst.Log.Mappings : ElasticConst.Trace.Mappings;
            foreach (var item in conditions)
            {
                var mapping = mappings.FirstOrDefault(m => string.Equals(m.Name, item.Name, StringComparison.OrdinalIgnoreCase));
                list.Add(CompareCondition<TResult>(mapping, item));
            }
        }

        if (list.Any())
            return queryContainer.Bool(boolQuery => boolQuery.Must(list));

        return queryContainer;
    }

    private static IEnumerable<FieldConditionDto> AddFilter<TQuery>(TQuery query) where TQuery : BaseRequestDto
    {
        if (query is LogRequestDto log)
        {
            return log.Conditions;
        }
        else if (query is TraceRequestDto trace)
        {
            var result = query.Conditions?.ToList() ?? new();
            if (!string.IsNullOrEmpty(trace.Service))
                result.Add(new FieldConditionDto
                {
                    Name = ElasticConst.ServiceName,
                    Value = trace.Service,
                    Type = ConditionTypes.Equal
                });
            if (!string.IsNullOrEmpty(trace.Instance))
                result.Add(new FieldConditionDto
                {
                    Name = ElasticConst.ServiceInstance,
                    Value = trace.Service,
                    Type = ConditionTypes.Equal
                });
            if (!string.IsNullOrEmpty(trace.Endpoint))
                result.Add(new FieldConditionDto
                {
                    Name = ElasticConst.Endpoint,
                    Value = $"*{trace.Service}*",
                    Type = ConditionTypes.Regex
                });
            if (!string.IsNullOrEmpty(trace.TraceId))
                result.Add(new FieldConditionDto
                {
                    Name = ElasticConst.TraceId,
                    Value = trace.TraceId,
                    Type = ConditionTypes.Equal
                });
            return result;
        }
        return query.Conditions;
    }

    private static Func<QueryContainerDescriptor<TResult>, QueryContainer> CompareCondition<TResult>(ElasticseacherMappingResponseDto? mapping, FieldConditionDto query) where TResult : class
    {
        string fieldName = (mapping == null ? query.Name : mapping.Name)!;
        string keyword = fieldName;
        if (mapping?.IsKeyword ?? false)
            keyword = $"{keyword}.keyword";

        var value = query.Value;

        switch (query.Type)
        {
            case ConditionTypes.Equal:
                return (container) => container.Match(f => f.Field(keyword).Query(value?.ToString()));
            case ConditionTypes.NotEqual:
                return (container) => !container.Match(f => f.Field(keyword).Query(value?.ToString()));
            case ConditionTypes.Great:
                return (container) => container.Range(f => f.Field(keyword).GreaterThan(Convert.ToDouble(value)));
            case ConditionTypes.Less:
                return (container) => container.Range(f => f.Field(keyword).LessThan(Convert.ToDouble(value)));
            case ConditionTypes.GreatEqual:
                return (container) => container.Range(f => f.Field(keyword).GreaterThanOrEquals(Convert.ToDouble(value)));
            case ConditionTypes.LessEqual:
                return (container) => container.Range(f => f.Field(keyword).LessThanOrEquals(Convert.ToDouble(value)));
            case ConditionTypes.In:
                return (container) => container.Terms(f => f.Field(keyword).Terms((IEnumerable<object>)value));
            case ConditionTypes.NotIn:
                return (container) => !container.Terms(f => f.Field(keyword).Terms((IEnumerable<object>)value));
            case ConditionTypes.Exists:
                return (container) => container.Exists(f => f.Field(fieldName));
            case ConditionTypes.NotExists:
                return (container) => !container.Exists(f => f.Field(fieldName));
            case ConditionTypes.Regex:
                return (container) => container.Wildcard(f => f.Field(fieldName).Value(value?.ToString()));
            case ConditionTypes.NotRegex:
                return (container) => !container.Wildcard(f => f.Field(fieldName).Value(value?.ToString()));
        }
        return (container) => container;
    }

    private static SortDescriptor<TResult> SortFn<TQuery, TResult>(SortDescriptor<TResult> container, TQuery query) where TQuery : BaseRequestDto where TResult : class
    {
        if (query.Sorts == null || !query.Sorts.Any())
            return container.Descending(ElasticConst.Log.Timestamp);
        foreach (var item in query.Sorts)
        {
            if (item.IsAsc ?? false)
                container.Ascending(item.Name);
            else
                container.Descending(item.Name);
        }
        return container;
    }

    private static PaginationDto<LogResponseDto> SetLogResult(ISearchResponse<object> response)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new LogResponseDtoConverter());
        var text = JsonSerializer.Serialize(response.Documents);

        return new PaginationDto<LogResponseDto>(response.Total, JsonSerializer.Deserialize<List<LogResponseDto>>(text, options)!);
    }

    private static PaginationDto<TraceResponseDto> SetTraceResult(ISearchResponse<object> response)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TraceResponseDtoConverter());
        var text = JsonSerializer.Serialize(response.Documents);

        return new PaginationDto<TraceResponseDto>(response.Total, JsonSerializer.Deserialize<List<TraceResponseDto>>(text, options)!);
    }

    private static IAggregationContainer AggregationFn(AggregationContainerDescriptor<object> aggContainer, SimpleAggregateRequestDto aggModel, bool isLog)
    {
        var mappings = isLog ? ElasticConst.Log.Mappings : ElasticConst.Trace.Mappings;
        var mapping = mappings.FirstOrDefault(m => string.Equals(m.Name, aggModel.Name, StringComparison.OrdinalIgnoreCase));
        string field=mapping?.Name??aggModel.Name;
        string keyword = field;
        if (mapping != null && (mapping.IsKeyword ?? false))
            keyword = $"{keyword}.keyword";

        switch (aggModel.Type)
        {
            case AggregateTypes.Count:
                {
                    aggContainer.ValueCount(aggModel.Alias ?? aggModel.Name, agg => agg.Field(keyword));
                }
                break;
            case AggregateTypes.Sum:
                {
                    aggContainer.Sum(aggModel.Alias ?? aggModel.Name, agg => agg.Field(field));
                }
                break;
            case AggregateTypes.Avg:
                {
                    aggContainer.Average(aggModel.Alias ?? aggModel.Name, agg => agg.Field(field));
                }
                break;
            case AggregateTypes.DistinctCount:
                {
                    aggContainer.Cardinality(aggModel.Alias ?? aggModel.Name, agg => agg.Field(keyword));
                }
                break;
            //case AggregateTypes.Histogram:
            //    {
            //        aggContainer.Histogram(aggModel.Alias, agg => agg.Field(aggModel.Name).Interval(new Time(aggModel.Interval).Milliseconds));
            //    }
            //    break;
            case AggregateTypes.DateHistogram:
                {
                    if (mapping != null && mapping.Type != "date")
                    {
                        throw new UserFriendlyException($"Field of type [{field}] is not supported for aggregation [date_histogram]");
                    }
                    aggContainer.DateHistogram(aggModel.Alias ?? aggModel.Name, agg => agg.Field(keyword).FixedInterval(new Time(aggModel.Interval)));
                }
                break;
            case AggregateTypes.GroupBy:
                {
                    aggContainer.Terms(aggModel.Alias ?? aggModel.Name, agg => agg.Field(keyword).Size(aggModel.MaxCount));
                }
                break;
        }

        return aggContainer;
    }

    private static object SetAggregationResult(ISearchResponse<object> response, SimpleAggregateRequestDto aggModel)
    {
        if (response.Aggregations == null || !response.Aggregations.Any())
            return default!;

        var result = new Dictionary<string, string>();
        foreach (var item in response.Aggregations)
        {
            if (aggModel.Type - AggregateTypes.DistinctCount <= 0 && item.Value is ValueAggregate value && value != null)
            {
                string temp = default!;
                if (!string.IsNullOrEmpty(value.ValueAsString))
                    temp = value.ValueAsString;
                else if (value.Value.HasValue)
                    temp = value.Value.Value.ToString();

                if (string.IsNullOrEmpty(temp))
                    continue;

                return temp;
            }
            else if (aggModel.Type == AggregateTypes.DateHistogram && item.Value is BucketAggregate bucketAggregate)
            {
                foreach (DateHistogramBucket bucket in bucketAggregate.Items)
                {
                    result.Add(bucket.KeyAsString, (bucket.DocCount ?? 0).ToString());
                }
            }
            else if (aggModel.Type == AggregateTypes.GroupBy && item.Value is BucketAggregate termsAggregate)
            {
                return termsAggregate.Items.Select(it => ((KeyedBucket<object>)it).Key.ToString()).ToList();
            }
        }
        return result;
    }
}
