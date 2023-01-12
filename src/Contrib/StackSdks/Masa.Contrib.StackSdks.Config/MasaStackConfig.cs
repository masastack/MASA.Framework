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

        public string Version => GetValue(MasaStackConfigConst.VERSION);

        public string Cluster => GetValue(MasaStackConfigConst.CLUSTER);

        public RedisModel? RedisModel
        {
            get
            {
                var redisStr = GetValue(MasaStackConfigConst.REDIS);
                var redisModel = JsonSerializer.Deserialize<RedisModel>(redisStr);

                return redisModel;
            }
        }

        public string IsDemo => GetValue(MasaStackConfigConst.IS_DEMO);

        public string TlsName => GetValue(MasaStackConfigConst.TLS_NAME);

        public List<string> ProjectList => this.GetAllServer().Keys.ToList();
    }
}
