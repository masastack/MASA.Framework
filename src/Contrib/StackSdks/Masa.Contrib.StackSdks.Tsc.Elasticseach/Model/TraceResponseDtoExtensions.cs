// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Model;

internal static class TraceResponseDtoExtensions
{
    public static bool TryParseHttp(this ElasticseachTraceResponseDto data, out ElasticseachTraceHttpResponseDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "http.method" }))
            return false;
        result = data.Attributes.ToObject<ElasticseachTraceHttpResponseDto>();

        result.RequestHeaders = data.Attributes.GroupByKeyPrefix("http.request.header.", ReadHeaderValues);
        result.ReponseHeaders = data.Attributes.GroupByKeyPrefix("http.response.header.", ReadHeaderValues);

        result.Name = data.Name;
        result.Status = data.TraceStatus;
        return true;
    }    

    public static bool TryParseDatabase(this ElasticseachTraceResponseDto data, out ElasticseachTraceDatabaseResponseDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "db.system" }))
            return false;
        result = data.Attributes.ToObject<ElasticseachTraceDatabaseResponseDto>();
        return true;
    }

    public static bool TryParseException(this ElasticseachTraceResponseDto data, out ElasticseachTraceExceptionResponseDto result)
    {
        result = default!;
        if (!data.Attributes.ContainsKeies(new string[] { "exception.type", "exception.message" }))
            return false;

        result = data.Attributes.ToObject<ElasticseachTraceExceptionResponseDto>();

        return true;
    }

    private static IEnumerable<string> ReadHeaderValues(object obj)
    {
        if (obj is JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.Array)
            {
                return value.EnumerateArray().Select(item => item.ToString()).ToArray();
            }
            else
            {
                return new string[] { value.ToString() };
            }
        }
        return new string[] { obj.ToString()! };
    }

    private static bool ContainsKeies(this Dictionary<string, object> dic, IEnumerable<string> keys)
    {
        return dic != null && dic.Any() && dic.Keys.Any(key => keys.Any(k => key == k));
    }    
}
