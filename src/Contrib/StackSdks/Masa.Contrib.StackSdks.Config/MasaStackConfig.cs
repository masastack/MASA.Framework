// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public class MasaStackConfig : IMasaStackConfig
{
    private readonly IConfigurationApiClient _configurationApiClient;

    private static ConcurrentDictionary<string, string> ConfigMap { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public MasaStackConfig() { }

    public MasaStackConfig(IConfigurationApiClient client, Dictionary<string, string> configs)
    {
        _configurationApiClient = client;
        ConfigMap = new(configs);
    }

    public RedisModel RedisModel
    {
        get
        {
            var redisStr = GetValue(MasaStackConfigConstant.REDIS);
            return JsonSerializer.Deserialize<RedisModel>(redisStr) ?? throw new JsonException();
        }
    }

    public ElasticModel ElasticModel
    {
        get
        {
            var redisStr = GetValue(MasaStackConfigConstant.ELASTIC);
            return JsonSerializer.Deserialize<ElasticModel>(redisStr) ?? throw new JsonException();
        }
    }

    public bool IsDemo => bool.Parse(GetValue(MasaStackConfigConstant.IS_DEMO));

    public string Version => GetValue(MasaStackConfigConstant.VERSION);

    public string Cluster => GetValue(MasaStackConfigConstant.CLUSTER);

    public string OtlpUrl => GetValue(MasaStackConfigConstant.OTLP_URL);

    public string DomainName => GetValue(MasaStackConfigConstant.DOMAIN_NAME);

    public string Environment => GetValue(MasaStackConfigConstant.ENVIRONMENT);

    public string Namespace => GetValue(MasaStackConfigConstant.NAMESPACE);

    public string AdminPwd => GetValue(MasaStackConfigConstant.ADMIN_PWD);

    public string DccSecret => GetValue(MasaStackConfigConstant.DCC_SECRET);

    public bool SingleSsoClient { get; }

    public string SuffixIdentity => GetValue(MasaStackConfigConstant.SUFFIX_IDENTITY);

    public List<string> GetProjectList() => this.GetAllServer().Keys.ToList();

    public string GetValue(string key)
    {
        GetValues().TryGetValue(key, out var value);
        return value ?? ConfigMap[key];
    }

    public virtual Dictionary<string, string> GetValues()
    {
        try
        {
            var remoteConfigs = _configurationApiClient.GetAsync<Dictionary<string, string>>(
               ConfigMap[MasaStackConfigConstant.ENVIRONMENT],
               ConfigMap[MasaStackConfigConstant.CLUSTER],
               DEFAULT_PUBLIC_ID,
               DEFAULT_CONFIG_NAME).ConfigureAwait(false).GetAwaiter().GetResult();
            return remoteConfigs;
        }
        catch (ArgumentException)
        {
            return new(ConfigMap);
        }
    }
}
