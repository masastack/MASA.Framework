// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Config.Tests")]
namespace Masa.Contrib.StackSdks.Config;

internal static class MasaStackConfigUtils
{
    public static DccOptions GetDefaultDccOptions(Dictionary<string, string> configMap)
    {
        var value = configMap.GetValueOrDefault(MasaStackConfigConstant.MASA_STACK);
        var data = JsonSerializer.Deserialize<JsonArray>(value) ?? new();
        var dccServerAddress = data.FirstOrDefault(i => i?["id"]?.ToString() == MasaStackConstant.DCC)?[MasaStackConstant.SERVICE]?["host"]?.ToString() ?? "";
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
