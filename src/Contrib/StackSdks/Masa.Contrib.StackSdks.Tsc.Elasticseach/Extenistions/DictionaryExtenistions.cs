// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.
[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests")]
namespace System.Collections.Generic;

internal static class DictionaryExtenistions
{
    public static Dictionary<string, T> ConvertDic<T>(this Dictionary<string, object> dic, string prefix, Func<object, T>? convert = null)
    {
        var result = new Dictionary<string, T>();
        foreach (var key in dic.Keys)
        {
            if (!key.StartsWith(prefix))
                continue;
            var value = dic[key];
            var newKey = key[prefix.Length..];
            if (convert != null)
                value = convert(dic[key]);
            result.Add(newKey, (T)value!);
        }
        return result;
    }
}
