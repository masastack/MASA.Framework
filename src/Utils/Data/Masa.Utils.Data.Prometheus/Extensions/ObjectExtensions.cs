// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public static class ObjectExtensions
{
    /// <summary>
    /// Currently supported types: class, struct and types implementing the IEnumerable interface,
    /// struct and class use public get properties and fields by default,
    /// The IEnumerable<KeyValuePair> type is directly converted to: key[]=value1&key[]=value2
    /// enum uses strings by default. If you need to use numeric values, please set isEnumString=false
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isEnumString"></param>
    /// <param name="isCamelCase"></param>
    /// <param name="isUrlEncode"></param>
    /// <returns></returns>
    public static string? ToUrlParam(this object? obj, bool isEnumString = true, bool isCamelCase = true, bool isUrlEncode = true)
    {
        return GetValue(obj, string.Empty, isEnumString, isCamelCase, isUrlEncode);
    }

    private static string? GetValue(object? obj, string preStr, bool isEnumString = false, bool isCamelCase = true, bool isUrlEncode = true)
    {
        if (obj == null) return null;
        var type = obj.GetType();
        if (type == typeof(string))
        {
            var str = (string)obj;
            return AppendValue(preStr, str, "=", isUrlEncode);
        }

        if (type.IsValueType)
        {
            if (type.IsEnum)
            {
                var str = isEnumString ? obj.ToString() : Convert.ToInt32(obj).ToString();
                return AppendValue(preStr, str, "=", isUrlEncode);
            }

            //sample value
            if (type.IsPrimitive)
            {
                var str = obj.ToString();
                return AppendValue(preStr, str, "=", isUrlEncode);
            }

            //struct           
            return GetObjValue(type, obj, preStr, isEnumString, isCamelCase, isUrlEncode);
        }

        if (type.IsArray || type.GetInterfaces().Any(t => t.Name.IndexOf("IEnumerable") == 0))
            return GetEnumerableValue(obj, preStr, isEnumString, isCamelCase, isUrlEncode);

        if (type.IsClass)
            return GetObjValue(type, obj, preStr, isEnumString, isCamelCase, isUrlEncode);

        //current type not suport
        return null;
    }

    private static string GetObjValue(Type type, object obj, string preStr, bool isEnumString = false, bool isCamelCase = true, bool isUrlEncode = true)
    {
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
        var list = new List<string>();

        foreach (var item in properties)
        {
            var str = GetMemberInfoValue(item, item.GetValue(obj), preStr, isEnumString, isCamelCase, isUrlEncode);
            if (string.IsNullOrEmpty(str))
                continue;
            list.Add(str);
        }

        foreach (var item in fields)
        {
            var str = GetMemberInfoValue(item, item.GetValue(obj), preStr, isEnumString, isCamelCase, isUrlEncode);
            if (string.IsNullOrEmpty(str))
                continue;
            list.Add(str);
        }

        if (!list.Any())
            return default!;

        list.Sort();
        return string.Join('&', list);
    }

    private static string? GetMemberInfoValue(MemberInfo info, object? value, string preStr, bool isEnumString = false, bool isCamelCase = true, bool isUrlEncode = true)
    {
        if (value == null)
            return null;

        var name = info.Name;
        if (isCamelCase)
            name = name.ToCamelCase();

        return GetValue(value, AppendValue(preStr, name, ".", isUrlEncode) ?? default!, isEnumString, isCamelCase, isUrlEncode);
    }

    private static string? GetEnumerableValue(object obj, string preStr, bool isEnumString = false, bool isCamelCase = true, bool isUrlEncode = true)
    {
        var list = new List<string>();
        foreach (var item in (IEnumerable)obj)
        {
            if (item is KeyValuePair<string, object> keyValue)
            {
                var name = keyValue.Key;
                if (isCamelCase)
                    name = name.ToCamelCase();
                var str = GetValue(keyValue.Value, AppendValue(preStr, name, ".", isUrlEncode) ?? default!, isEnumString, isCamelCase, isUrlEncode);
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);
            }
            else
            {
                var str = GetValue(item, $"{preStr}{(isUrlEncode ? HttpUtility.UrlEncode("[]", Encoding.UTF8) : "[]")}", isEnumString, isCamelCase, isUrlEncode);
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);
            }
        }
        if (!list.Any())
            return default!;

        list.Sort();
        return string.Join('&', list);
    }

    private static string? AppendValue(string preStr, string? value, string splitChar, bool isUrlEncode)
    {
        if (string.IsNullOrEmpty(preStr) || string.IsNullOrEmpty(value))
            return value;

        if (isUrlEncode)
            return $"{preStr}{splitChar}{HttpUtility.UrlEncode(value, Encoding.UTF8)}";
        else
            return $"{preStr}{splitChar}{value}";
    }
}
