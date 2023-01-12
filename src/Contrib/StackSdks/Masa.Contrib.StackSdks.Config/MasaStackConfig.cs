// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Collections.Concurrent;

namespace Masa.Contrib.StackSdks.Config
{
    public class MasaStackConfig : IMasaStackConfig
    {

        private ConcurrentDictionary<string, string> ConfigMap { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public string GetValue(string key) => GetValue(key, () => string.Empty);

        public string GetValue(string key, Func<string> defaultFunc)
        {
            if (ConfigMap.ContainsKey(key)) return ConfigMap[key];
            return defaultFunc.Invoke();
        }

        public void SetValue(string key, string value) => ConfigMap[key] = value;
    }
}
