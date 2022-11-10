// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Utils.Data.Prometheus.Test")]
namespace Masa.Utils.Data.Prometheus;

internal class MasaPrometheusClient : IMasaPrometheusClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger _logger;
    private const string LABLES_URL = "/api/v1/labels";
    private const string QUERY_URL = "/api/v1/query";
    private const string QUERY_RANGE_URL = "/api/v1/query_range";
    private const string SERIES_URL = "/api/v1/series";
    private const string EXEMPLAR_URL = "/api/v1/query_exemplars";
    private const string LABLE_VALUE_URL = "/api/v1/label/{0}/values";
    private const string METRIC_META_URL = "/api/v1/meta";

    public MasaPrometheusClient(HttpClient client, JsonSerializerOptions jsonSerializerOptions, ILogger<MasaPrometheusClient> logger)
    {
        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
        _logger = logger;
    }

    public async Task<ExemplarResultResponse> ExemplarQueryAsync(QueryExemplarRequest query)
    {
        return await QueryDataAsync<ExemplarResultResponse>(EXEMPLAR_URL, query);
    }

    public async Task<LabelResultResponse> LabelsQueryAsync(MetaDataQueryRequest query)
    {
        return await QueryDataAsync<LabelResultResponse>(LABLES_URL, query);
    }

    public async Task<LabelResultResponse> LabelValuesQueryAsync(LableValueQueryRequest query)
    {
        return await QueryDataAsync<LabelResultResponse>(string.Format(LABLE_VALUE_URL, query.Lable), query);
    }

    public async Task<QueryResultCommonResponse> QueryAsync(QueryRequest query)
    {
        return await QueryDataAsync<QueryResultCommonResponse>(QUERY_URL, query);
    }

    public async Task<QueryResultCommonResponse> QueryRangeAsync(QueryRangeRequest query)
    {
        return await QueryDataAsync<QueryResultCommonResponse>(QUERY_RANGE_URL, query);
    }

    public async Task<SeriesResultResponse> SeriesQueryAsync(MetaDataQueryRequest query)
    {
        return await QueryDataAsync<SeriesResultResponse>(SERIES_URL, query);
    }

    public async Task<MetaResultResponse> MetricMetaQueryAsync(MetricMetaQueryRequest query)
    {
        return await QueryDataAsync<MetaResultResponse>(METRIC_META_URL, query);
    }

    private async Task<T> QueryDataAsync<T>(string url, object data) where T : ResultBaseResponse
    {
        var str = await _client.GetAsync(url, data, _logger);
        if (string.IsNullOrEmpty(str))
            return default!;

        var baseResult = JsonSerializer.Deserialize<T>(str, _jsonSerializerOptions);

        if (baseResult == null || baseResult.Status != ResultStatuses.Success)
        {
            return baseResult ?? default!;
        }

        if (typeof(T) == typeof(QueryResultCommonResponse))
        {
            var result = baseResult as QueryResultCommonResponse;
            if (result == null || result.Data == null)
                return baseResult;
            switch (result.Data.ResultType)
            {
                case ResultTypes.Matrix:
                    {
                        var temp = JsonSerializer.Serialize(result.Data.Result, _jsonSerializerOptions);
                        result.Data.Result = JsonSerializer.Deserialize<QueryResultMatrixRangeResponse[]>(temp, _jsonSerializerOptions);
                        if (result.Data.Result != null && result.Data.Result.Any())
                        {
                            foreach (QueryResultMatrixRangeResponse item in result.Data.Result)
                            {
                                if (item.Values == null || !item.Values.Any())
                                    continue;
                                var array = item.Values.ToArray();
                                int i = 0, max = array.Length - 1;
                                do
                                {
                                    array[i] = ConvertJsonToObjValue(array[i]);
                                    i++;
                                }
                                while (max - i >= 0);
                                item.Values = array;
                            }
                        }
                        return result as T ?? default!;
                    }
                case ResultTypes.Vector:
                    {
                        var temp = JsonSerializer.Serialize(result.Data.Result, _jsonSerializerOptions);
                        result.Data.Result = JsonSerializer.Deserialize<QueryResultInstantVectorResponse[]>(temp, _jsonSerializerOptions);
                        if (result.Data.Result != null && result.Data.Result.Any())
                        {
                            foreach (QueryResultInstantVectorResponse item in result.Data.Result)
                            {
                                item.Value = ConvertJsonToObjValue(item.Value);
                            }
                        }
                        return result as T ?? default!;
                    }
                default:
                    {
                        if (result.Data.Result != null && result.Data.Result.Any())
                        {
                            result.Data.Result = ConvertJsonToObjValue(result.Data.Result);
                        }
                    }
                    break;
            }
        }

        return baseResult;
    }


    private static object[] ConvertJsonToObjValue(object[]? values)
    {
        if (values == null || values.Length - 2 < 0)
            return default!;

        return new object[] { ((JsonElement)values[0]).GetDouble(), ((JsonElement)values[1]).GetString() ?? default! };
    }
}
