// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Model;

internal static class TraceResponseDtoExtensions
{
    private static readonly string[] httpKeys = new string[] { "http.method" };
    private static readonly string[] databaseKeys = new string[] { "db.system" };
    private static readonly string[] exceptionKeys = new string[] { "exception.type", "exception.message" };

    public static bool TryParseHttp(this ElasticseachTraceResponseDto data, out ElasticseachTraceHttpResponseDto result)
    {
        result = default!;
        if (!IsContainsAnyKey(data.Attributes, httpKeys))
            return false;
        result = data.Attributes.ConvertTo<ElasticseachTraceHttpResponseDto>();

        result.RequestHeaders = data.Attributes.GroupByKeyPrefix("http.request.header.", ReadHeaderValues);
        result.ReponseHeaders = data.Attributes.GroupByKeyPrefix("http.response.header.", ReadHeaderValues);

        result.Name = data.Name;
        result.Status = data.TraceStatus;
        return true;
    }

    public static bool TryParseDatabase(this ElasticseachTraceResponseDto data, out ElasticseachTraceDatabaseResponseDto result)
    {
        result = default!;
        if (!IsContainsAnyKey(data.Attributes, databaseKeys))
            return false;
        result = data.Attributes.ConvertTo<ElasticseachTraceDatabaseResponseDto>();
        return true;
    }

    public static bool TryParseException(this ElasticseachTraceResponseDto data, out ElasticseachTraceExceptionResponseDto result)
    {
        result = default!;
        if (!IsContainsAnyKey(data.Attributes, exceptionKeys))
            return false;

        result = data.Attributes.ConvertTo<ElasticseachTraceExceptionResponseDto>();

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

    private static bool IsContainsAnyKey(Dictionary<string, object> source, string[] keys)
    {
        if (source == null || !source.Any() || keys == null || !keys.Any())
            return false;
        if (keys.Length == 1)
            return source.ContainsKey(keys[0]);

        return keys.Any(k => source.ContainsKey(k));
    }
}
