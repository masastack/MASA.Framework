// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config
{
    public static class MasaStackConfigUtils
    {
        public static JsonArray GetAllServer(Dictionary<string, string> configMap)
        {
            var value = configMap.GetValueOrDefault(MasaStackConfigConstant.MASA_STACK);
            if (string.IsNullOrEmpty(value))
            {
                return new();
            }
            return JsonSerializer.Deserialize<JsonArray>(value) ?? new();
        }

        public static string GetServerDomain(Dictionary<string, string> configMap, string project, string service)
        {
            var domain = "";
            var jsonObject = GetAllServer(configMap).FirstOrDefault(jNode => jNode?["id"]?.ToString() == project);
            if (jsonObject != null)
            {
                var secondaryDomain = jsonObject[service]?.ToString();
                domain = jsonObject[service]?.ToString() ?? "";
            }
            return domain;
        }

        public static string GetDccServiceDomain(Dictionary<string, string> configMap)
        {
            return GetServerDomain(configMap, MasaStackConstant.DCC, MasaStackConstant.SERVICE);
        }

        public static DccOptions GetDefaultDccOptions(Dictionary<string, string> configMap)
        {
            var dccServerAddress = GetDccServiceDomain(configMap);
            var redisStr = configMap.GetValueOrDefault(MasaStackConfigConstant.REDIS) ?? throw new Exception("redis options can not null");
            var redis = JsonSerializer.Deserialize<RedisModel>(redisStr) ?? throw new JsonException();
            var secret = configMap.GetValueOrDefault(MasaStackConfigConstant.DCC_SECRET);

            var options = new DccOptions
            {
                Environment = configMap.GetValueOrDefault(MasaStackConfigConstant.ENVIRONMENT)!,
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
