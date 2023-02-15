// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public class MasaStackConfigOptions
{
    public static ConcurrentDictionary<string, string> ConfigMap { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public static string GetValue(string key) => GetValue(key, () => string.Empty);

    public static string GetValue(string key, Func<string> defaultFunc)
    {
        if (ConfigMap.ContainsKey(key)) return ConfigMap[key];
        return defaultFunc.Invoke();
    }

    public static void SetValue(string key, string value) => ConfigMap[key] = value;

    public static void SetValues(Dictionary<string, string> configMap)
    {
        foreach (var config in configMap)
        {
            SetValue(config.Key, config.Value);
        }
    }
}
