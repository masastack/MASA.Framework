// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest;

internal static class IElasticClientExtenstion
{
    public static async Task SearchAsync<TResult, TQuery>(this IElasticClient client,
        string indexName,
        TQuery query,
        Func<SearchDescriptor<TResult>, SearchDescriptor<TResult>> searchDescriptorFunc,
        Action<ISearchResponse<TResult>, TQuery> resultFunc) where TResult : class where TQuery : class
    {
        try
        {
            if (resultFunc is null)
                return;
            var searchResponse = await client.SearchAsync<TResult>(s => searchDescriptorFunc(s.Index(indexName)));
            searchResponse.FriendlyElasticException();
            if (searchResponse.IsValid)
            {
                resultFunc.Invoke(searchResponse, query);
            }
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"SearchAsync execute error {ex.Message}");
        }
    }

    private static SearchDescriptor<T> AddPageSize<T>(this SearchDescriptor<T> container, bool hasPage, int page, int size) where T : class
    {
        if (!hasPage)
            return container.Size(size);

        var start = (page - 1) * size;

        if (ElasticConst.MaxRecordCount - start - size <= 0)
            throw new UserFriendlyException($"elastic query data max count must be less {ElasticConst.MaxRecordCount}, please input more condition to limit");

        return container.Size(size).From(start);
    }

    private static SearchDescriptor<TResult> AddCondition<TResult, TQuery>(this SearchDescriptor<TResult> searchDescriptor,
        Func<QueryContainerDescriptor<TResult>, TQuery, QueryContainer> condition,
        TQuery query) where TResult : class where TQuery : class
    {
        if (condition is not null)
            searchDescriptor = searchDescriptor.Query(queryContainer => condition.Invoke(queryContainer, query));
        return searchDescriptor;
    }

    private static SearchDescriptor<TResult> AddSort<TResult, TQuery>(this SearchDescriptor<TResult> searchDescriptor,
        Func<SortDescriptor<TResult>, TQuery, SortDescriptor<TResult>> sort,
        TQuery query) where TResult : class where TQuery : class
    {

        if (sort != null)
            searchDescriptor = searchDescriptor.Sort(sortDescriptor => sort(sortDescriptor, query));

        return searchDescriptor;
    }

    private static SearchDescriptor<TResult> AddAggregate<TResult, TQuery>(this SearchDescriptor<TResult> searchDescriptor,
        Func<AggregationContainerDescriptor<TResult>, TQuery, IAggregationContainer> aggregate,
        TQuery query) where TResult : class where TQuery : class
    {
        if (aggregate != null)
        {
            searchDescriptor = searchDescriptor.Aggregations(agg => aggregate.Invoke(agg, query));
        }
        return searchDescriptor;
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
    public static async Task<PaginationDto<LogResponseDto>> SearchLogAsync(this IElasticClient client, BaseRequestDto query)
    {
        PaginationDto<LogResponseDto> result = default!;
        await client.SearchAsync(ElasticConst.Log.IndexName, query,
        (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition((searchDescriptor, query) => SearchFn(searchDescriptor, query, true), query).AddSort(SortFn, query).AddPageSize(true, query.Page, query.Size),
        (response, q) => result = SetLogResult(response));
        return result;
    }

    public static async Task<object> AggregateLogAsync(this IElasticClient client, SimpleAggregateRequestDto query)
    {
        object result = default!;
        await client.SearchAsync(ElasticConst.Log.IndexName, query,
       (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition((searchDescriptor, query) => SearchFn(searchDescriptor, query, true), query).AddSort(SortFn, query).AddPageSize(false, 0, 0).AddAggregate((agg, query) => AggregationFn(agg, query, true), query),
       (response, q) => result = SetAggregationResult(response, q));
        return result;
    }
    #endregion

    #region trace
    public static async Task<PaginationDto<TraceResponseDto>> SearchTraceAsync(this IElasticClient client, BaseRequestDto query)
    {
        PaginationDto<TraceResponseDto> result = default!;
        await client.SearchAsync(ElasticConst.Trace.IndexName, query,
        (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition((searchDescriptor, query) => SearchFn(searchDescriptor, query, false), query).AddSort(SortFn, query).AddPageSize(true, query.Page, query.Size),
        (response, q) => result = SetTraceResult(response));
        return result;
    }

    public static async Task<object> AggregateTraceAsync(this IElasticClient client, SimpleAggregateRequestDto query)
    {
        object result = default!;
        await client.SearchAsync(ElasticConst.Trace.IndexName, query,
       (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition((searchDescriptor, query) => SearchFn(searchDescriptor, query, false), query).AddSort(SortFn, query).AddPageSize(false, 0, 0).AddAggregate((agg, query) => AggregationFn(agg, query, false), query),
       (response, q) => result = SetAggregationResult(response, q));
        return result;
    }
    #endregion

    private static QueryContainer SearchFn<TQuery, TResult>(QueryContainerDescriptor<TResult> queryContainer, TQuery query, bool isLog) where TQuery : BaseRequestDto where TResult : class
    {
        var list = new List<Func<QueryContainerDescriptor<TResult>, QueryContainer>>();
        string timestamp = isLog ? ElasticConst.Log.Timestamp : ElasticConst.Trace.Timestamp;
        var mappings = isLog ? ElasticConst.Log.Mappings : ElasticConst.Trace.Mappings;

        if (!string.IsNullOrEmpty(query.RawQuery))
        {
            list.Add(queryContainer => queryContainer.Raw(query.RawQuery));
        }
        if (query.Start > DateTime.MinValue && query.End > DateTime.MinValue && query.Start < query.End)
        {
            list.Add(queryContainer => queryContainer.DateRange(dateRangeQuery => dateRangeQuery.GreaterThanOrEquals(query.Start).LessThanOrEquals(query.End).Field(timestamp)));
        }
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            list.Add(queryContainer => queryContainer.QueryString(queryString => queryString.Query(query.Keyword)));
        }

        query.AppendConditions();
        var conditions = AddFilter(query);
        if (conditions != null && conditions.Any())
        {
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
        var result = query.Conditions?.ToList() ?? new();
        if (!string.IsNullOrEmpty(query.Service))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConst.ServiceName,
                Value = query.Service,
                Type = ConditionTypes.Equal
            });
        if (!string.IsNullOrEmpty(query.Instance))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConst.ServiceInstance,
                Value = query.Service,
                Type = ConditionTypes.Equal
            });
        if (!string.IsNullOrEmpty(query.Endpoint))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConst.Endpoint,
                Value = $"*{query.Service}*",
                Type = ConditionTypes.Regex
            });
        if (!string.IsNullOrEmpty(query.TraceId))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConst.TraceId,
                Value = query.TraceId,
                Type = ConditionTypes.Equal
            });
        return result;
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
        string field = mapping?.Name ?? aggModel.Name;
        string aliasName = aggModel.Alias ?? aggModel.Name;
        string keyword = field;
        if (mapping != null && (mapping.IsKeyword ?? false))
            keyword = $"{keyword}.keyword";

        switch (aggModel.Type)
        {
            case AggregateTypes.Count:
                {
                    aggContainer.ValueCount(aliasName, agg => agg.Field(keyword));
                }
                break;
            case AggregateTypes.Sum:
                {
                    aggContainer.Sum(aliasName, agg => agg.Field(field));
                }
                break;
            case AggregateTypes.Avg:
                {
                    aggContainer.Average(aliasName, agg => agg.Field(field));
                }
                break;
            case AggregateTypes.DistinctCount:
                {
                    aggContainer.Cardinality(aliasName, agg => agg.Field(keyword));
                }
                break;
            case AggregateTypes.DateHistogram:
                {
                    if (mapping != null && mapping.Type != "date")
                    {
                        throw new UserFriendlyException($"Field of type [{field}] is not supported for aggregation [date_histogram]");
                    }
                    aggContainer.DateHistogram(aliasName, agg => agg.Field(keyword).FixedInterval(new Time(aggModel.Interval)));
                }
                break;
            case AggregateTypes.GroupBy:
                {
                    aggContainer.Terms(aliasName, agg => agg.Field(keyword).Size(aggModel.MaxCount));
                }
                break;
        }

        return aggContainer;
    }

    private static object SetAggregationResult(ISearchResponse<object> response, SimpleAggregateRequestDto aggModel)
    {
        if (response.Aggregations == null || !response.Aggregations.Any())
            return default!;

        foreach (var item in response.Aggregations.Values)
        {
            if (item is ValueAggregate value)
            {
                return GetDouble(value);
            }
            else if (item is BucketAggregate bucketAggregate)
            {
                return GetBucketValue(bucketAggregate, aggModel.Type);
            }
        }
        return default!;
    }

    private static double GetDouble(ValueAggregate value)
    {
        return value.Value ?? default;
    }

    private static object GetBucketValue(BucketAggregate value, AggregateTypes type)
    {
        if (type == AggregateTypes.GroupBy)
            return value.Items.Select(it => ((KeyedBucket<object>)it).KeyAsString).ToList();
        else if (type == AggregateTypes.DateHistogram)
        {
            var result = new Dictionary<double, long>();
            foreach (var bucket in value.Items)
            {
                var dateHistogramBucket = (DateHistogramBucket)bucket;
                result.Add(dateHistogramBucket.Key, (dateHistogramBucket.DocCount ?? 0));
            }
            return result;
        }
        return default!;
    }
}
