// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach;

internal class TraceService : ITraceService
{
    private readonly IElasticClient _client;
    private readonly ICallerFactory _callerFactory;

    public TraceService(IElasticsearchFactory elasticsearchFactory, ICallerFactory callerFactory)
    {
        _client = elasticsearchFactory.CreateElasticClient();
        _callerFactory = callerFactory;
    }

    public async Task<object> AggregateAsync(SimpleAggregateRequestDto query)
    {
        return await _client.AggregateTraceAsync(query);
    }

    public async Task<IEnumerable<TraceResponseDto>> GetAsync(string traceId)
    {
        return (await _client.SearchTraceAsync(new BaseRequestDto { TraceId = traceId, Page = 1, Size = ElasticConst.MaxRecordCount - 1 })).Items;
    }

    public Task<PaginationDto<TraceResponseDto>> ListAsync(BaseRequestDto query)
    {
        return _client.SearchTraceAsync(query);
    }   
}