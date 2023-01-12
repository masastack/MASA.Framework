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

    public string Version => GetValue(MasaStackConfigConst.VERSION);

    public string Cluster => GetValue(MasaStackConfigConst.CLUSTER);

    public string OtlpUrl => GetValue(MasaStackConfigConst.OTLP_URL);

    public List<string> ProjectList() => this.GetAllServer().Keys.ToList();

    public string GetValue(string key) => _options.Value.GetValue(key);

    public void SetValue(string key, string value) => _options.Value.SetValue(key, value);
}
