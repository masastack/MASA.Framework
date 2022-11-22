// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Service;

public class LogService : ILogService
{
    private readonly ICaller _caller;
    internal const string AGGREGATE_URI = "/api/log/aggregate";
    internal const string LATEST_URI = "/api/log/latest";
    internal const string MAPPING_URI = "/api/log/mapping";

    public LogService(ICaller caller)
    {
        _caller = caller;
    }   

    public async Task<TResult> GetAggregationAsync<TResult>(SimpleAggregateRequestDto query)
    {
        var str = await _caller.GetByBodyAsync<string>(AGGREGATE_URI, query);
        if (string.IsNullOrEmpty(str))
            return default!;
        if(str is TResult t)
            return t;
        return JsonSerializer.Deserialize<TResult>(str)!;
    }

    public async Task<IEnumerable<MappingResponseDto>> GetMappingAsync()
    {
        return (await _caller.GetAsync<IEnumerable<MappingResponseDto>>(MAPPING_URI))!;
    }

    public async Task<LogResponseDto> GetLatestAsync(LogLatestRequest query)
    {
        return (await _caller.GetByBodyAsync<LogResponseDto>(LATEST_URI, query))!;
    }
}
