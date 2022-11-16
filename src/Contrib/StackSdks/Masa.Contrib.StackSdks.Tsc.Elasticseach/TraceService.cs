// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Log.Elasticseach;

internal class TraceService : ITraceService
{
    private readonly IElasticClient _client;

    public TraceService(IElasticsearchFactory elasticsearchFactory)
    {
        _client = elasticsearchFactory.CreateElasticClient(false);
    }

    public async Task<object> AggregateAsync(SimpleAggregateRequestDto query)
    {
        return await _client.AggregateTraceAsync(query);
    }    

    public async Task<IEnumerable<TraceResponseDto>> GetAsync(string traceId)
    {
        var result = await _client.SearchTraceAsync(new TraceRequestDto { TraceId = traceId, Page=1, Size = ElasticConst.MaxRecordCount - 1 });
        return (IEnumerable<TraceResponseDto>)result.Items ?? Array.Empty<TraceResponseDto>();
    }

    public Task<PaginationDto<TraceResponseDto>> ListAsync(TraceRequestDto query)
    {
        return _client.SearchTraceAsync(query);
    }
}
