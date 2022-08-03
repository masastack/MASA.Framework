// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Utils.Data.Prometheus.Test")]
namespace Masa.Utils.Data.Prometheus;

internal class MasaPrometheusClient : IMasaPrometheusClient
{
    private readonly ICallerProvider _caller;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public MasaPrometheusClient(ICallerProvider caller, JsonSerializerOptions jsonSerializerOptions)
    {
        _caller = caller;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task<ExemplarResultResponse> ExemplarQueryAsync(QueryExemplarRequest query)
    {
        return await QueryDataAsync<ExemplarResultResponse>("/api/v1/query_exemplars", query);
    }

    public async Task<LabelResultResponse> LabelsQueryAsync(MetaDataQueryRequest query)
    {
        return await QueryDataAsync<LabelResultResponse>("/api/v1/labels", query);
    }

    public async Task<LabelResultResponse> LabelValuesQueryAsync(LableValueQueryRequest query)
    {
        var name = query.Lable;
        query.Lable = null;
        return await QueryDataAsync<LabelResultResponse>($"/api/v1/label/{name}/values", query);
    }

    public async Task<QueryResultCommonResponse> QueryAsync(QueryRequest query)
    {
        return await QueryDataAsync<QueryResultCommonResponse>("/api/v1/query", query);
    }

    public async Task<QueryResultCommonResponse> QueryRangeAsync(QueryRangeRequest query)
    {
        return await QueryDataAsync<QueryResultCommonResponse>("/api/v1/query_range", query);
    }

    public async Task<SeriesResultResponse> SeriesQueryAsync(MetaDataQueryRequest query)
    {
        return await QueryDataAsync<SeriesResultResponse>("/api/v1/series", query);
    }

    private async Task<T> QueryDataAsync<T>(string url, object data) where T : ResultBaseResponse
    {
        var str = await _caller.GetAsync(url, data);
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
