// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public class MasaStackConfig : IMasaStackConfig
{
    public MasaStackConfig(Dictionary<string, string> configMap, IConfigurationApiClient? client)
    {
        if (client is not null)
        {
            var configs = client.GetAsync<Dictionary<string, string>>(
                Environment,
                Cluster,
                DEFAULT_PUBLIC_ID,
                DEFAULT_CONFIG_NAME).ConfigureAwait(false).GetAwaiter().GetResult();

            MasaStackConfigOptions.SetValues(configs);
        }

        if (configMap.Any())
        {
            MasaStackConfigOptions.SetValues(configMap);
        }
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

    public string TlsName => GetValue(MasaStackConfigConstant.TLS_NAME);

    public string Version => GetValue(MasaStackConfigConstant.VERSION);

    public string Cluster => GetValue(MasaStackConfigConstant.CLUSTER);

    public string OtlpUrl => GetValue(MasaStackConfigConstant.OTLP_URL);

    public string DomainName => GetValue(MasaStackConfigConstant.DOMAIN_NAME);

    public string Environment => GetValue(MasaStackConfigConstant.ENVIRONMENT);

    public string Namespace => GetValue(MasaStackConfigConstant.NAMESPACE);

    public string AdminPwd => GetValue(MasaStackConfigConstant.ADMIN_PWD);

    public string DccSecret => GetValue(MasaStackConfigConstant.DCC_SECRET);

    public bool SingleSsoClient { get; }

    public List<string> GetProjectList() => this.GetAllServer().Keys.ToList();

    public string GetValue(string key) => MasaStackConfigOptions.GetValue(key);

    public void SetValue(string key, string value) => MasaStackConfigOptions.SetValue(key, value);

    public void SetValues(Dictionary<string, string> configMap) => MasaStackConfigOptions.SetValues(configMap);
}
