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
            if (resultFunc is null) return;
            ISearchResponse<TResult> searchResponse;
            if (query is ElasticsearchScrollRequestDto scrollQuery)
                searchResponse = string.IsNullOrEmpty(scrollQuery.ScrollId) ?
                                            await client.SearchAsync<TResult>(s => searchDescriptorFunc(s.Index(indexName)).Scroll(scrollQuery.Scroll)) :
                                            await client.ScrollAsync<TResult>(scrollQuery.Scroll, scrollQuery.ScrollId);
            else
                searchResponse = await client.SearchAsync<TResult>(s => searchDescriptorFunc(s.Index(indexName)));

            searchResponse.FriendlyElasticException();
            if (searchResponse.IsValid)
                resultFunc.Invoke(searchResponse, query);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"SearchAsync execute error", ex);
        }
    }

    #region mapping

    public static async Task<IEnumerable<MappingResponseDto>> GetMappingAsync(this ICaller caller, string indexName, CancellationToken token = default)
    {
        var path = $"/{indexName}/_mapping";
        var result = await caller.GetAsync<object>(path, token);
        var json = (JsonElement)result!;
        foreach (var item in json.EnumerateObject())
        {
            JsonElement mapping;
            if (item.Value.TryGetProperty("mappings", out mapping))
            {
                var mappings = GetRepProperties(mapping, default!)!;
                if (mappings != null && mappings.Any())
                    return mappings;
            }
        }

        throw new UserFriendlyException($"can't find mapping for index: {indexName}");
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
        if (value.TryGetProperty(MappingConstant.PROPERTY, out JsonElement result))
            return result;
        return null;
    }

    private static string? GetType(JsonElement value)
    {
        if (value.TryGetProperty(MappingConstant.TYPE, out JsonElement element))
            return element.ToString();
        return default;
    }

    private static void SetKeyword(JsonElement value, ElasticseacherMappingResponseDto model)
    {
        if (value.TryGetProperty(MappingConstant.FIELD, out JsonElement fields) &&
       fields.TryGetProperty(MappingConstant.KEYWORD, out JsonElement element))
        {
            if (element.TryGetProperty(MappingConstant.TYPE, out JsonElement type) && type.ToString() == MappingConstant.KEYWORD)
                model.IsKeyword = true;
            if (element.TryGetProperty(MappingConstant.MAXLENGTH, out JsonElement maxLength))
                model.MaxLenth = maxLength.GetInt32();
        }
    }

    public static void FriendlyElasticException<T>(this ISearchResponse<T> response) where T : class
    {
        if (!response.IsValid)
            throw new UserFriendlyException($"elastic query error: status: {response.ServerError?.Status}, message: {response.OriginalException?.Message ?? response.ServerError?.ToString()}, DebugInformation: {response.DebugInformation}");
    }

    #endregion

    #region log

    public static async Task<PaginatedListBase<LogResponseDto>> SearchLogAsync(this IElasticClient client, BaseRequestDto query)
    {
        PaginatedListBase<LogResponseDto> result = default!;
        await client.SearchAsync(ElasticConstant.Log.IndexName, query,
        (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition(query, true)
        .AddSort(query)
        .AddPageSize(query, true),
        (response, q) => result = SetLogResult(response));
        return result;
    }

    public static async Task<object> AggregateLogAsync(this IElasticClient client, SimpleAggregateRequestDto query)
    {
        object result = default!;
        await client.SearchAsync(ElasticConstant.Log.IndexName, query,
       (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition(query, true)
       .AddSort(query)
       .AddPageSize(query, false)
       .AddAggregate(query, true),
       (response, q) => result = SetAggregationResult(response, q));
        return result;
    }

    #endregion

    #region trace
    public static async Task<PaginatedListBase<TraceResponseDto>> SearchTraceAsync(this IElasticClient client, BaseRequestDto query)
    {
        PaginatedListBase<TraceResponseDto> result = default!;
        await client.SearchAsync(ElasticConstant.Trace.IndexName, query,
        (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition(query, false)
        .AddSort(query, false)
        .AddPageSize(query, true),
        (response, q) =>
        {
            result = q is ElasticsearchScrollRequestDto ? SetTraceScrollResult(response) : SetTraceResult(response);
        });
        return result;
    }

    public static async Task<object> AggregateTraceAsync(this IElasticClient client, SimpleAggregateRequestDto query)
    {
        object result = default!;
        await client.SearchAsync(ElasticConstant.Trace.IndexName, query,
       (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition(query, false)
       .AddSort(query, false)
       .AddPageSize(query, false)
       .AddAggregate(query, false),
       (response, q) => result = SetAggregationResult(response, q));
        return result;
    }

    public static async Task<string> GetMaxDelayTraceIdAsync(this IElasticClient client, BaseRequestDto query)
    {
        string traceId = default!;
        await client.SearchAsync(ElasticConstant.Trace.IndexName, query,
        (SearchDescriptor<object> searchDescriptor) => searchDescriptor.AddCondition(query, false)
        .Sort(sort => sort.Script(script => script.Order(SortOrder.Descending).Script(s => s.Source("doc['EndTimestamp'].value.toEpochSecond()-doc['@timestamp'].value.toEpochSecond()")).Type("number")))
        .Size(1),
        (response, q) =>
        {
            var result = SetTraceResult(response);
            traceId = result.Result.FirstOrDefault()?.TraceId!;
        });

        return traceId;
    }
    #endregion

    #region set condition    

    private static SearchDescriptor<TResult> AddCondition<TQuery, TResult>(this SearchDescriptor<TResult> searchDescriptor, TQuery query, bool isLog) where TQuery : BaseRequestDto where TResult : class
    {
        var list = new List<Func<QueryContainerDescriptor<TResult>, QueryContainer>>();
        string timestamp = isLog ? ElasticConstant.Log.Timestamp : ElasticConstant.Trace.Timestamp;
        var mappings = isLog ? ElasticConstant.Log.Mappings.Value : ElasticConstant.Trace.Mappings.Value;

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
            list.Add(queryContainer => queryContainer.QueryString(queryString => queryString.Query(query.Keyword).DefaultOperator(Operator.And)));
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
        {
            return searchDescriptor.Query(container => container.Bool(boolQuery => boolQuery.Must(list)));
        }

        return searchDescriptor;
    }

    private static IEnumerable<FieldConditionDto> AddFilter<TQuery>(TQuery query) where TQuery : BaseRequestDto
    {
        var result = query.Conditions?.ToList() ?? new();
        if (!string.IsNullOrEmpty(query.Service))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConstant.ServiceName,
                Value = query.Service,
                Type = ConditionTypes.Equal
            });
        if (!string.IsNullOrEmpty(query.Instance))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConstant.ServiceInstance,
                Value = query.Instance,
                Type = ConditionTypes.Equal
            });
        if (!string.IsNullOrEmpty(query.Endpoint))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConstant.Endpoint,
                Value = $"*{query.Endpoint}*",
                Type = ConditionTypes.Regex
            });
        if (!string.IsNullOrEmpty(query.TraceId))
            result.Add(new FieldConditionDto
            {
                Name = ElasticConstant.TraceId,
                Value = query.TraceId,
                Type = ConditionTypes.Equal
            });
        return result;
    }

    private static Func<QueryContainerDescriptor<TResult>, QueryContainer> CompareCondition<TResult>(ElasticseacherMappingResponseDto? mapping, FieldConditionDto query) where TResult : class
    {
        CreateFieldKeyword(query.Name, mapping, out var fieldName, out var keyword);
        var value = query.Value;
        bool isDate = value is DateTime;

        switch (query.Type)
        {
            case ConditionTypes.Equal:
                return (container) => container.Match(f => f.Field(keyword).Query(value?.ToString()));
            case ConditionTypes.NotEqual:
                return (container) => !container.Match(f => f.Field(keyword).Query(value?.ToString()));
            case ConditionTypes.Great:
                if (isDate)
                    return (container) => container.DateRange(f => f.Field(keyword).GreaterThan((DateTime)value));
                return (container) => container.Range(f => f.Field(keyword).GreaterThan(Convert.ToDouble(value)));
            case ConditionTypes.Less:
                if (isDate)
                    return (container) => container.DateRange(f => f.Field(keyword).LessThan((DateTime)value));
                return (container) => container.Range(f => f.Field(keyword).LessThan(Convert.ToDouble(value)));
            case ConditionTypes.GreatEqual:
                if (isDate)
                    return (container) => container.DateRange(f => f.Field(keyword).GreaterThanOrEquals((DateTime)value));
                return (container) => container.Range(f => f.Field(keyword).GreaterThanOrEquals(Convert.ToDouble(value)));
            case ConditionTypes.LessEqual:
                if (isDate)
                    return (container) => container.DateRange(f => f.Field(keyword).LessThanOrEquals((DateTime)value));
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

    #endregion

    #region set page

    private static SearchDescriptor<TResult> AddPageSize<TResult, TQuery>(this SearchDescriptor<TResult> container, TQuery query, bool hasPage) where TResult : class where TQuery : BaseRequestDto
    {
        if (query.Page <= 0)
            query.Page = 1;
        if (query.PageSize <= 0)
            query.PageSize = 20;
        if (query is ElasticsearchScrollRequestDto || !hasPage)
            return container.Size(query.PageSize);

        var start = (query.Page - 1) * query.PageSize;
        if (ElasticConstant.MaxRecordCount - start - query.PageSize <= 0)
            throw new UserFriendlyException($"elastic query data max count must be less {ElasticConstant.MaxRecordCount}, please input more condition to limit");
        return container.Size(query.PageSize).From(start);

    }
    #endregion

    #region set sort   

    private static SearchDescriptor<TResult> AddSort<TQuery, TResult>(this SearchDescriptor<TResult> searchDescriptor, TQuery query, bool isLog = true) where TQuery : BaseRequestDto where TResult : class
    {
        LogTraceSetting setting = isLog ? ElasticConstant.Log : ElasticConstant.Trace;

        if (query.Sort == null)
            return searchDescriptor.Sort(sort => sort.Descending(setting.Timestamp));

        var mapping = setting.Mappings.Value.FirstOrDefault(m => string.Equals(m.Name, query.Sort.Name, StringComparison.OrdinalIgnoreCase));
        CreateFieldKeyword(query.Sort.Name, mapping, out var field, out var keyword);

        Func<SortDescriptor<TResult>, SortDescriptor<TResult>> sortFunc;
        if (query.Sort.IsDesc)
            sortFunc = (sort) => sort.Descending(keyword);
        else
            sortFunc = (sort) => sort.Ascending(keyword);

        return searchDescriptor.Sort(sort => sortFunc(sort));
    }

    #endregion

    #region set result    
    private static PaginatedListBase<LogResponseDto> SetLogResult(ISearchResponse<object> response)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new LogResponseDtoConverter());
        var text = JsonSerializer.Serialize(response.Documents);

        return new PaginatedListBase<LogResponseDto> { Total = response.Total, Result = JsonSerializer.Deserialize<List<LogResponseDto>>(text, options)! };
    }

    private static PaginatedListBase<TraceResponseDto> SetTraceResult(ISearchResponse<object> response)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TraceResponseDtoConverter());
        var text = JsonSerializer.Serialize(response.Documents);
        return new PaginatedListBase<TraceResponseDto> { Total = response.Total, Result = JsonSerializer.Deserialize<List<TraceResponseDto>>(text, options)! };
    }

    private static ElasticsearchScrollResponseDto<TraceResponseDto> SetTraceScrollResult(ISearchResponse<object> response)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TraceResponseDtoConverter());
        var text = JsonSerializer.Serialize(response.Documents);
        return new ElasticsearchScrollResponseDto<TraceResponseDto> { Total = response.Total, Result = JsonSerializer.Deserialize<List<TraceResponseDto>>(text, options)!, ScrollId = response.ScrollId };
    }

    #endregion

    #region aggregate    

    private static SearchDescriptor<TResult> AddAggregate<TResult, TQuery>(this SearchDescriptor<TResult> searchDescriptor, TQuery aggModel, bool isLog) where TResult : class where TQuery : SimpleAggregateRequestDto
    {
        Func<AggregationContainerDescriptor<TResult>, IAggregationContainer>? aggregateFunc = null;
        var mappings = isLog ? ElasticConstant.Log.Mappings.Value : ElasticConstant.Trace.Mappings.Value;
        var mapping = mappings.FirstOrDefault(m => string.Equals(m.Name, aggModel.Name, StringComparison.OrdinalIgnoreCase));
        CreateFieldKeyword(aggModel.Name, mapping, out var field, out var keyword);
        string aliasName = aggModel.Alias ?? field;

        switch (aggModel.Type)
        {
            case AggregateTypes.Count:
                {
                    aggregateFunc = (aggContainer) => aggContainer.ValueCount(aliasName, agg => agg.Field(keyword));
                }
                break;
            case AggregateTypes.Sum:
                {
                    aggregateFunc = (aggContainer) => aggContainer.Sum(aliasName, agg => agg.Field(field));
                }
                break;
            case AggregateTypes.Avg:
                {
                    aggregateFunc = (aggContainer) => aggContainer.Average(aliasName, agg => agg.Field(field));
                }
                break;
            case AggregateTypes.DistinctCount:
                {
                    aggregateFunc = (aggContainer) => aggContainer.Cardinality(aliasName, agg => agg.Field(keyword));
                }
                break;
            case AggregateTypes.DateHistogram:
                {
                    if (mapping != null && mapping.Type != "date")
                    {
                        throw new UserFriendlyException($"Field of type [{field}] is not supported for aggregation [date_histogram]");
                    }
                    aggregateFunc = (aggContainer) => aggContainer.DateHistogram(aliasName, agg => agg.Field(keyword).FixedInterval(new Time(aggModel.Interval)));
                }
                break;
            case AggregateTypes.GroupBy:
                {
                    aggregateFunc = (aggContainer) => aggContainer.Terms(aliasName, agg => agg.Field(keyword).Size(aggModel.MaxCount));
                }
                break;
        }
        if (aggregateFunc != null)
            searchDescriptor = searchDescriptor.Aggregations(aggContainer => aggregateFunc(aggContainer));

        return searchDescriptor;
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
                return GetBucketValue(bucketAggregate, aggModel.Type, aggModel.AllValue);
            }
        }
        return default!;
    }

    private static double GetDouble(ValueAggregate value)
    {
        return value.Value ?? default;
    }

    private static object GetBucketValue(BucketAggregate value, AggregateTypes type, bool isAll)
    {
        if (type == AggregateTypes.GroupBy)
        {
            if (isAll)
                return value.Items.Select(it => KeyValuePair.Create(((KeyedBucket<object>)it).Key.ToString(), ((KeyedBucket<object>)it).DocCount)).ToList();
            else
                return value.Items.Select(it => ((KeyedBucket<object>)it).Key.ToString()).ToList();
        }

        else if (type == AggregateTypes.DateHistogram)
        {
            var result = new List<KeyValuePair<double, long>>();
            foreach (var bucket in value.Items)
            {
                var dateHistogramBucket = (DateHistogramBucket)bucket;
                result.Add(KeyValuePair.Create(dateHistogramBucket.Key, (dateHistogramBucket.DocCount ?? 0)));
            }
            return result;
        }
        return default!;
    }

    private static void CreateFieldKeyword(string name, ElasticseacherMappingResponseDto? mapping, out string field, out string keyword)
    {
        if (mapping == null)
        {
            field = name;
            keyword = name;
        }
        else
        {
            field = mapping.Name;
            if (mapping.Type == "text" && mapping.IsKeyword.HasValue && mapping.IsKeyword.Value)
                keyword = $"{field}.keyword";
            else
                keyword = field;
        }
    }

    #endregion
}
