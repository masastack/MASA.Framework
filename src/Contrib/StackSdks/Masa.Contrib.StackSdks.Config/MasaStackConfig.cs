// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public class MasaStackConfig : IMasaStackConfig
{
    private readonly IOptions<MasaStackConfigOptions> _options;

    public MasaStackConfig(IOptions<MasaStackConfigOptions> options)
    {
        _options = options;
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

    public bool SingleSsoClient { get; }

    public List<string> ProjectList() => this.GetAllServer().Keys.ToList();

    public string GetValue(string key) => _options.Value.GetValue(key);

    public void SetValue(string key, string value) => _options.Value.SetValue(key, value);
}
