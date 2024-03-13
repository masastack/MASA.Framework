// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System.Collections.Generic;

public static class DictionaryExtenistions
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public static Dictionary<string, T> GroupByKeyPrefix<T>(this Dictionary<string, object> source, string prefix, Func<object, T>? convertFunc = null)
    {
        var result = new Dictionary<string, T>();
        foreach (var key in source.Keys)
        {
            if (!key.StartsWith(prefix))
                continue;
            var value = source[key];
            var newKey = key[prefix.Length..];
            if (convertFunc != null)
                value = convertFunc(source[key]);
            result.Add(newKey, (T)value!);
        }
        return result;
    }

    internal static T ConvertTo<T>(this Dictionary<string, object> dic)
    {
        var text = JsonSerializer.Serialize(dic, _serializerOptions);
        return JsonSerializer.Deserialize<T>(text, _serializerOptions)!;
    }
}
