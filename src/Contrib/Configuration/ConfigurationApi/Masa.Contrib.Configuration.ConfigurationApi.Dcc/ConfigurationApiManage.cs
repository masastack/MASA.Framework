// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

public class ConfigurationApiManage : ConfigurationApiBase, IConfigurationApiManage
{
    private readonly ICaller _caller;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ConfigurationApiManage(
        ICaller caller,
        DccSectionOptions defaultSectionOption,
        JsonSerializerOptions jsonSerializerOptions,
        List<DccSectionOptions>? expandSectionOptions)
        : base(defaultSectionOption, expandSectionOptions)
    {
        _caller = caller;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    ///<inheritdoc/>
    public async Task AddAsync(string environment, string cluster, string appId,
        Dictionary<string, object> configObjects,
        bool isEncryption = false)
    {
        var newConfigObjects = configObjects.ToDictionary(k => k.Key, v => JsonSerializer.Serialize(v.Value, _jsonSerializerOptions));

        var requestUri = $"open-api/releasing/{GetEnvironment(environment)}/{GetCluster(cluster)}/{GetAppId(appId)}/{isEncryption}";
        var result = await _caller.PostAsync(requestUri, newConfigObjects, default).ConfigureAwait(false);

        // 299 is the status code when throwing a UserFriendlyException in masa.framework
        if ((int)result.StatusCode == 299 || !result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpRequestException(error);
        }
    }

    public async Task UpdateAsync(string environment, string cluster, string appId, string configObject, object value)
    {
        var requestUri = $"open-api/releasing/{GetEnvironment(environment)}/{GetCluster(cluster)}/{GetAppId(appId)}/{GetConfigObject(configObject)}";
        var result = await _caller.PutAsync(requestUri, JsonSerializer.Serialize(value, _jsonSerializerOptions), default).ConfigureAwait(false);

        // 299 is the status code when throwing a UserFriendlyException in masa.framework
        if ((int)result.StatusCode == 299 || !result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpRequestException(error);
        }
    }
}
