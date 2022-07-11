// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Tsc.Service;

internal class MetricService : IMetricService
{
    private readonly ICallerProvider _caller;
    internal const string RANGEVALUES_URL = "/api/metric/range-values";
    internal const string NAMES_URI = "/api/metric/names";
    internal const string LABELVALUES_URI = "/api/metric/label-values";

    public MetricService(ICallerProvider caller)
    {
        _caller = caller;
    }

    public async Task<IEnumerable<string>> GetNamesAsync(IEnumerable<string>? matches = default)
    {
        string param = default!;
        if (matches != null && matches.Any(s => !string.IsNullOrEmpty(s)))
        {
            param = string.Join(',', matches.Where(s => !string.IsNullOrEmpty(s)));
        }
        return (await _caller.GetAsync<IEnumerable<string>>(NAMES_URI, new { match = param })) ?? default!;
    }

    public async Task<Dictionary<string, List<string>>> GetLabelValuesAsync(LableValuesRequest query)
    {
        var data = await _caller.GetByBodyAsync<Dictionary<string, Dictionary<string, List<string>>>>(LABELVALUES_URI, query);
        if (data == null || !data.ContainsKey(query.Match))
            return default!;

        return data[query.Match];
    }

    public async Task<string> GetValuesAsync(ValuesRequest query)
    {
        if (query.Lables != null && !string.IsNullOrEmpty(query.Match))
        {
            query.Match = $"{query.Match}{{{string.Join(',', query.Lables)}}}";
        }

        return (await _caller.GetByBodyAsync<string>(RANGEVALUES_URL, query)) ?? default!;
    }
}
