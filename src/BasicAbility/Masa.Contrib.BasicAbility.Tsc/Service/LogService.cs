// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Tsc.Service;

public class LogService : ILogService
{
    private const string AGGREGATION_URI = "/api/log/aggregation";
    private const string LATEST_URI = "/api/log/latest";
    private const string FIELD_URI = "/api/log/field";
    private readonly ICallerProvider _caller;

    public LogService(ICallerProvider caller)
    {
        _caller = caller;
    }

    public async Task<IEnumerable<KeyValuePair<string, string>>> GetAggregationAsync(LogAggregationRequest query)
    {
        return (await _caller.GetAsync<IEnumerable<KeyValuePair<string, string>>>(AGGREGATION_URI, query)) ?? default!;
    }

    public async Task<IEnumerable<string>> GetFieldsAsync()
    {
        return (await _caller.GetAsync<IEnumerable<string>>(FIELD_URI)) ?? default!;
    }

    public async Task<object> GetLatestAsync(LogAggregationRequest query)
    {
        return (await _caller.GetAsync<object>(LATEST_URI, query)) ?? default!;
    }
}
