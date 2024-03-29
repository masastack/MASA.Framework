// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Config.Tests")]
namespace Masa.Contrib.StackSdks.Config;

internal static class MasaStackConfigUtils
{
    public static DccOptions GetDefaultDccOptions(Dictionary<string, string> configMap, MasaStackProject project, MasaStackApp app)
    {
        var data = GetMasaStackJsonArray(configMap);
        var dccServerAddress = data.FirstOrDefault(i => i?["id"]?.ToString() == MasaStackProject.DCC.Name)?[MasaStackApp.Service.Name]?["domain"]?.ToString() ?? "";
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
            ConfigObjectSecret = secret,
            AppId = GetAppId(configMap, project, app)
        };

        return options;
    }

    static string GetAppId(Dictionary<string, string> configMap, MasaStackProject project, MasaStackApp app)
    {
        var data = GetMasaStackJsonArray(configMap);
        return data.FirstOrDefault(i => i?["id"]?.ToString() == project.Name)?[app.Name]?["id"]?.ToString() ?? "";
    }

    static JsonArray GetMasaStackJsonArray(Dictionary<string, string> configMap)
    {
        var value = configMap.GetValueOrDefault(MasaStackConfigConstant.MASA_STACK) ?? "";
        return JsonSerializer.Deserialize<JsonArray>(value) ?? new();
    }
}
