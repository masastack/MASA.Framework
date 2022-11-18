// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.
[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests")]
namespace System.Collections.Generic;

internal static class DictionaryExtenistions
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
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

    public static T ToObject<T>(this Dictionary<string, object> dic)
    {
        var text = JsonSerializer.Serialize(dic, _serializerOptions);
        return JsonSerializer.Deserialize<T>(text, _serializerOptions)!;
    }
}
