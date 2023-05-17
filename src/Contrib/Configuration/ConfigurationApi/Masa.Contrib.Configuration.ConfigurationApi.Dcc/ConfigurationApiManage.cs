// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal class ConfigurationApiManage : ConfigurationApiBase, IConfigurationApiManage
{
    private readonly ICaller _caller;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ConfigurationApiManage(
        ICaller caller,
        JsonSerializerOptions jsonSerializerOptions,
        DccConfigurationOptions dccConfigurationOptions)
        : base(dccConfigurationOptions)
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
        var requestUri =
            $"open-api/releasing/{GetEnvironment(environment)}/{GetCluster(cluster)}/{GetAppId(appId)}/{GetConfigObject(configObject)}";
        var result = await _caller.PutAsync(requestUri, JsonSerializer.Serialize(value, _jsonSerializerOptions), default)
            .ConfigureAwait(false);

        // 299 is the status code when throwing a UserFriendlyException in masa.framework
        if ((int)result.StatusCode == 299 || !result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpRequestException(error);
        }
    }

    /// <summary>
    /// Initialize config object
    /// </summary>
    /// <param name="environment">Environment name</param>
    /// <param name="cluster">Cluster name</param>
    /// <param name="appId">App id</param>
    /// <param name="configObjects">Config objects,Key:config object name,Value:config object content</param>
    /// <param name="isEncryption">Config object content whether to encrypt</param>
    /// <returns></returns>
    public void Add(string environment, string cluster, string appId, Dictionary<string, object> configObjects, bool isEncryption = false)
    {
        var newConfigObjects = configObjects.ToDictionary(k => k.Key, v => JsonSerializer.Serialize(v.Value, _jsonSerializerOptions));

        var requestUri = $"open-api/releasing/{GetEnvironment(environment)}/{GetCluster(cluster)}/{GetAppId(appId)}/{isEncryption}";
        var result = _caller.PostAsync(requestUri, newConfigObjects, default).ConfigureAwait(false).GetAwaiter().GetResult();

        // 299 is the status code when throwing a UserFriendlyException in masa.framework
        if ((int)result.StatusCode != 299 && result.IsSuccessStatusCode)
            return;

        var error = result.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        throw new HttpRequestException(error);
    }

    public void Update(string environment, string cluster, string appId, string configObject, object value)
    {
        var requestUri =
            $"open-api/releasing/{GetEnvironment(environment)}/{GetCluster(cluster)}/{GetAppId(appId)}/{GetConfigObject(configObject)}";
        var result =  _caller.PutAsync(requestUri, JsonSerializer.Serialize(value, _jsonSerializerOptions), default)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        // 299 is the status code when throwing a UserFriendlyException in masa.framework
        if ((int)result.StatusCode != 299 && result.IsSuccessStatusCode)
            return;

        var error = result.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        throw new HttpRequestException(error);
    }
}
