// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.
[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests")]
namespace System;

internal static class JsonElementExtensions
{
    public static IEnumerable<KeyValuePair<string, object>> ToKeyValuePairs(this JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.Object)
            return default!;

        return GetObject(value);
    }

    public static object GetValue(this JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                return GetObject(value);
            case JsonValueKind.Array:
                return GetArray(value);
            case JsonValueKind.String:
                return value.GetString()!;
            case JsonValueKind.Number:
                return GetNumber(value);
            case JsonValueKind.True:
            case JsonValueKind.False:
                return value.GetBoolean();
            default:
                return default!;
        }
    }

    private static object GetNumber(JsonElement value)
    {
        var str = value.GetRawText();

        if (Regex.IsMatch(str, @"\."))
        {
            return value.GetDouble();
        }
        else
        {
            if (!value.TryGetInt32(out int num))
                return value.GetInt64();
            return num;
        }
    }

    private static IEnumerable<KeyValuePair<string, object>> GetObject(JsonElement value)
    {
        var result = new Dictionary<string, object>();
        foreach (var item in value.EnumerateObject())
        {
            var v = GetValue(item.Value);
            if (v == null)
                continue;
            result.Add(item.Name, v);
        }
        if (result.Any())
            return result;
        return default!;
    }

    public static IEnumerable<object?> GetArray(this JsonElement value)
    {
        var temp = value.EnumerateArray();
        if (!temp.Any())
            return default!;
        var list = new List<object?>();
        foreach (var item in temp)
        {
            var v = GetValue(item);
            list.Add(v);
        }
        return list;
    }    
}
