// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Log.Elasticseach.Model;

internal static class TraceResponseDtoExtensions
{
    private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static bool IsHttp(this ElasticseachTraceResponseDto data, out ElasticseachTraceHttpResponseDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "http.method" }))
            return false;
        result = data.Attributes.Deserialize<ElasticseachTraceHttpResponseDto>();

        result.RequestHeaders = data.Attributes.ConvertDic("http.request.header.", ToStringEnumerable);
        result.ReponseHeaders = data.Attributes.ConvertDic("http.response.header.", ToStringEnumerable);

        result.Name = data.Name;
        result.Status = data.TraceStatus;
        return true;
    }

    private static IEnumerable<string> ToStringEnumerable(object obj)
    {
        var value = (JsonElement)obj;

        if (value.ValueKind == JsonValueKind.Array)
        {
            return value.EnumerateArray().Select(item => item.ToString()).ToArray();
        }
        else
        {
            return new string[] { value.ToString() };
        }
    }

    public static bool IsDatabase(this ElasticseachTraceResponseDto data, out ElasticseachTraceDatabaseResponseDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "db.system" }))
            return false;
        result = data.Attributes.Deserialize<ElasticseachTraceDatabaseResponseDto>();
        return true;
    }

    public static bool IsException(this ElasticseachTraceResponseDto data, out ElasticseachTraceExceptionResponseDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "exception.type", "exception.message" }))
            return false;

        result = data.Attributes.Deserialize<ElasticseachTraceExceptionResponseDto>();

        return true;
    }

    private static bool ContainsKeies(this Dictionary<string, object> dic, IEnumerable<string> keys)
    {
        return dic != null && dic.Any() && dic.Keys.Any(key => keys.Any(k => key == k));
    }

    private static T Deserialize<T>(this Dictionary<string, object> dic)
    {
        var text = JsonSerializer.Serialize(dic, _serializerOptions);
        return JsonSerializer.Deserialize<T>(text, _serializerOptions)!;
    }
}
