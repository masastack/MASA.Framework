// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch;

internal class TraceService : ITraceService
{
    private readonly IElasticClient _client;

    public TraceService(IElasticClientFactory elasticClientFactory)
    {
        _client = elasticClientFactory.CreateElasticClient(false);
    }

    public async Task<object> AggregateAsync(SimpleAggregateRequestDto query)
    {
        return await _client.AggregateTraceAsync(query);
    }

    public async Task<IEnumerable<TraceResponseDto>> GetAsync(string traceId)
    {
        return (await _client.SearchTraceAsync(new BaseRequestDto { TraceId = traceId, Page = 1, PageSize = ElasticConstant.MaxRecordCount - 1 })).Result;
    }

    public Task<string> GetMaxDelayTraceIdAsync(BaseRequestDto query)
    {
        return _client.GetMaxDelayTraceIdAsync(query);
    }

    public Task<PaginatedListBase<TraceResponseDto>> ListAsync(BaseRequestDto query)
    {
        return _client.SearchTraceAsync(query);
    }

    public Task<PaginatedListBase<TraceResponseDto>> ScrollAsync(BaseRequestDto query)
    {
        if (query is not ElasticsearchScrollRequestDto)
            throw new UserFriendlyException("parameter: query must is type: ElasticsearchScrollRequestDto");
        return _client.SearchTraceAsync(query);
    }
}
