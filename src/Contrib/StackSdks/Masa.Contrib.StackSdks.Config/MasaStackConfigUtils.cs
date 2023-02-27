// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config
{
    public static class MasaStackConfigUtils
    {
        public static Dictionary<string, JsonObject> GetAllServer(Dictionary<string, string> configMap)
        {
            var value = configMap.GetValueOrDefault(MasaStackConfigConstant.MASA_SERVER);
            if (string.IsNullOrEmpty(value))
            {
                return new();
            }
            return JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(value) ?? new();
        }

        public static string GetServerDomain(Dictionary<string, string> configMap, string protocol, string project, string service)
        {
            var domain = "";
            GetAllServer(configMap).TryGetValue(project, out JsonObject? jsonObject);
            if (jsonObject != null)
            {
                var secondaryDomain = jsonObject[service]?.ToString();
                if (secondaryDomain != null)
                {
                    domain = $"{protocol}://{secondaryDomain}.{configMap.GetValueOrDefault(MasaStackConfigConstant.NAMESPACE)}";
                }
            }
            return domain;
        }

        public static string GetDccServiceDomain(Dictionary<string, string> configMap)
        {
            return GetServerDomain(configMap, HttpProtocol.HTTP, MasaStackConstant.DCC, MasaStackConstant.SERVER);
        }

        public static DccOptions GetDefaultDccOptions(Dictionary<string, string> configMap)
        {
            var dccServerAddress = GetDccServiceDomain(configMap);
            var redisStr = configMap.GetValueOrDefault(MasaStackConfigConstant.REDIS) ?? throw new Exception("redis options can not null");
            var redis = JsonSerializer.Deserialize<RedisModel>(redisStr) ?? throw new JsonException();
            var secret = configMap.GetValueOrDefault(MasaStackConfigConstant.DCC_SECRET);

            var options = new DccOptions
            {
                ManageServiceAddress = dccServerAddress,
                RedisOptions = new Caching.Distributed.StackExchangeRedis.RedisConfigurationOptions
                {
                    Servers = new List<Caching.Distributed.StackExchangeRedis.RedisServerOptions>
                {
                    new Caching.Distributed.StackExchangeRedis.RedisServerOptions(redis.RedisHost,redis.RedisPort)
                },
                    DefaultDatabase = redis.RedisDb,
                    Password = redis.RedisPassword
                },
                PublicSecret = secret,
                ConfigObjectSecret = secret
            };

            return options;
        }
    }
}
