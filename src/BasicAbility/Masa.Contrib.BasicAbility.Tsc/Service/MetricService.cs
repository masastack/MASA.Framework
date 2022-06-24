// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Tsc.Service;

internal class MetricService : IMetricService
{
    private readonly ICallerProvider _caller;
    private const string AGGREGATION_URI = "/api/metric/aggregation";
    private const string METRIC_URI = "/api/metric/all";
    private const string LABELVALUES_URI = "/api/metric/label-values";

    public MetricService(ICallerProvider caller)
    {
        _caller = caller;
    }

    public async Task<IEnumerable<string>> GetMetricsAsync(IEnumerable<string>? match)
    {
        return (await _caller.GetAsync<IEnumerable<string>>(METRIC_URI, new { match })) ?? default!;
    }

    public async Task<Dictionary<string, List<string>>> GetLabelAndValuesAsync(MetricLableValuesRequest query)
    {
        var data = await _caller.GetByBodyAsync<Dictionary<string, Dictionary<string, List<string>>>>(LABELVALUES_URI, query);
        if (data == null || !data.ContainsKey(query.Match))
            return default!;

        return data[query.Match];
    }

    public async Task<string> GetMetricAggAsync(MetricAggRequest query)
    {
        return (await _caller.GetByBodyAsync<string>(AGGREGATION_URI, query)) ?? default!;
    }
}
