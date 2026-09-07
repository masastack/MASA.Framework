// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

public class ConfigurationApiClient : ConfigurationApiClientBase
{
    private readonly IMultilevelCacheClient _client;

    public ConfigurationApiClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        DccOptions dccOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(
            defaultSectionOption,
            expandSectionOptions,
            jsonSerializerOptions,
            dccOptions.ConfigObjectSecret,
            serviceProvider.GetService<ILogger<ConfigurationApiClient>>())
    {
        var client = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
    }

    protected override async Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsync(string key,
        Action<string>? valueChanged)
    {
        var publishRelease = await _client.GetAsync<PublishReleaseModel>(key, value =>
        {
            var result = FormatRaw(value, key);
            valueChanged?.Invoke(result.Raw);
        }).ConfigureAwait(false);

        return FormatRaw(publishRelease, key);
    }

    protected override string FomartKey(string environment, string cluster, string appId, string configObject)
        => $"{GetEnvironment(environment)}-{GetCluster(cluster)}-{GetAppId(appId)}-{GetConfigObject(configObject)}".ToLower();
}
