// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch;

internal class LogService : ILogService
{
    private readonly IElasticClient _client;
    private readonly ICallerFactory _callerFactory;

    public LogService(IElasticClientFactory elasticClientFactory, ICallerFactory callerFactory)
    {
        _client = elasticClientFactory.CreateElasticClient(true);
        _callerFactory = callerFactory;
    }

    public async Task<object> AggregateAsync(SimpleAggregateRequestDto query)
    {
        return await _client.AggregateLogAsync(query);
    }

    public async Task<PaginatedListBase<LogResponseDto>> ListAsync(BaseRequestDto query)
    {
        return await _client.SearchLogAsync(query);
    }

    public async Task<IEnumerable<MappingResponseDto>> GetMappingAsync()
    {
        return await _callerFactory.Create(true).GetMappingAsync(ElasticConstant.Log.IndexName);
    }
}
