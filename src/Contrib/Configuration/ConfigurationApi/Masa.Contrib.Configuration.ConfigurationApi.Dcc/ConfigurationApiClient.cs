// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal class ConfigurationApiClient : ConfigurationApiBase, IConfigurationApiClient
{
    private readonly IManualMultilevelCacheClient _multilevelCacheClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JsonSerializerOptions _dynamicJsonSerializerOptions;
    private readonly ILogger<ConfigurationApiClient>? _logger;

    private readonly ConcurrentDictionary<string, Lazy<Task<ExpandoObject>>> _taskExpandoObjectsByAsync = new();
    private readonly ConcurrentDictionary<string, Lazy<ExpandoObject>> _taskExpandoObjectsBySync = new();

    private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _taskJsonObjectsByAsync = new();
    private readonly ConcurrentDictionary<string, Lazy<object>> _taskJsonObjectsBySync = new();
    private readonly Masa.BuildingBlocks.Data.ISerializer _yamlSerializer;
    private readonly Masa.BuildingBlocks.Data.IDeserializer _yamlDeserializer;

    public ConfigurationApiClient(
        IManualMultilevelCacheClient multilevelCacheClient,
        JsonSerializerOptions jsonSerializerOptions,
        DccConfigurationOptions dccConfigurationOptions,
        ILogger<ConfigurationApiClient>? logger = null)
        : base(dccConfigurationOptions)
    {
        ArgumentNullException.ThrowIfNull(multilevelCacheClient);

        _multilevelCacheClient = multilevelCacheClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _dynamicJsonSerializerOptions = new JsonSerializerOptions(_jsonSerializerOptions);
        _dynamicJsonSerializerOptions.EnableDynamicTypes();
        _logger = logger;
        _yamlSerializer = new DefaultYamlSerializer(new SerializerBuilder().JsonCompatible().Build());
        _yamlDeserializer = new DefaultYamlDeserializer(new DeserializerBuilder().Build());
    }

    #region async

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string configObject, Action<string>? valueChanged)
    {
        return GetRawAsync(GetEnvironment(string.Empty), GetCluster(string.Empty), GetAppId(string.Empty), configObject, valueChanged);
    }

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string environment, string cluster, string appId,
        string configObject, Action<string>? valueChanged)
    {
        var key = FormatKey(environment, cluster, appId, configObject);
        return GetRawByKeyAsync(key, valueChanged, GetAppId(appId));
    }

    public Task<T> GetAsync<T>(string configObject, Action<T>? valueChanged)
    {
        return GetAsync(GetEnvironment(), GetCluster(), GetAppId(), configObject, valueChanged);
    }

    public async Task<T> GetAsync<T>(string environment, string cluster, string appId, string configObject, Action<T>? valueChanged = null)
    {
        var key = FormatKey(environment, cluster, appId, configObject);

        var value = await _taskJsonObjectsByAsync.GetOrAdd(key, k => new Lazy<Task<object>>(async () =>
        {
            var result = await GetRawByKeyAsync(k, (value) =>
            {
                var result = JsonSerializer.Deserialize<T>(value, _dynamicJsonSerializerOptions);

                var newValue = new Lazy<Task<object>>(() => Task.FromResult((object)result!));
                _taskJsonObjectsByAsync.AddOrUpdate(k, newValue, (_, _) => newValue);
                valueChanged?.Invoke(result!);
            }, GetSecret(appId)).ConfigureAwait(false);
            if (typeof(T).GetInterfaces().Any(type => type == typeof(IConvertible)))
            {
                if (result.ConfigurationType == ConfigurationTypes.Text)
                    return Convert.ChangeType(result.Raw, typeof(T));

                throw new FormatException(result.Raw);
            }

            return JsonSerializer.Deserialize<T>(result.Raw, _dynamicJsonSerializerOptions) ??
                throw new MasaException($"The content of [{configObject}] is wrong");
        })).Value.ConfigureAwait(false);

        return (T)value;
    }

    public Task<dynamic> GetDynamicAsync(
        string environment,
        string cluster,
        string appId,
        string configObject,
        Action<dynamic>? valueChanged = null)
    {
        var key = FormatKey(environment, cluster, appId, configObject);

        return GetDynamicAsync(key, (k, value, options) =>
        {
            var result = JsonSerializer.Deserialize<ExpandoObject>(value, options);
            var newValue = new Lazy<Task<ExpandoObject?>>(() => Task.FromResult(result)!);
            _taskExpandoObjectsByAsync.AddOrUpdate(k, newValue!, (_, _) => newValue!);
            valueChanged?.Invoke(result!);
        }, GetSecret(appId));
    }

    public Task<dynamic> GetDynamicAsync(string configObject)
    {
        var appId = GetAppId();
        var key = FormatKey(GetEnvironment(), GetCluster(), appId, configObject);
        return GetDynamicAsync(key, null, GetSecret(appId));
    }

    internal Task<dynamic> GetDynamicAsync(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged, string secret)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return GetDynamicInternalAsync(key, valueChanged, secret);
    }

    private async Task<dynamic> GetDynamicInternalAsync(
        string key,
        Action<string, dynamic, JsonSerializerOptions>? valueChanged,
        string secret)
    {
        var value = _taskExpandoObjectsByAsync.GetOrAdd(key, k => new Lazy<Task<ExpandoObject>>(async () =>
        {
            var raw = await GetRawByKeyAsync(k, value =>
            {
                valueChanged?.Invoke(k, value, _dynamicJsonSerializerOptions);
            }, secret).ConfigureAwait(false);
            return JsonSerializer.Deserialize<ExpandoObject>(raw.Raw, _dynamicJsonSerializerOptions) ?? throw new ArgumentException(key);
        })).Value.ConfigureAwait(false);

        return await value;
    }

    internal async Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsync(
        string key,
        Action<string>? valueChanged,
        string secret)
    {
        var publishRelease = await _multilevelCacheClient.GetAsync<PublishReleaseModel>(key, value =>
        {
            var result = FormatRaw(value, key, secret);
            valueChanged?.Invoke(result.Raw);
        }).ConfigureAwait(false);

        return FormatRaw(publishRelease, key, secret);
    }

    #endregion

    #region sync

    public (string Raw, ConfigurationTypes ConfigurationType) GetRaw(string configObject, Action<string>? valueChanged = null)
    {
        return GetRaw(GetEnvironment(string.Empty), GetCluster(string.Empty), GetAppId(string.Empty), configObject, valueChanged);
    }

    public (string Raw, ConfigurationTypes ConfigurationType) GetRaw(
        string environment,
        string cluster,
        string appId,
        string configObject,
        Action<string>? valueChanged = null)
    {
        var key = FormatKey(environment, cluster, appId, configObject);
        return GetRawByKey(key, valueChanged, GetAppId(appId));
    }

    public T Get<T>(string configObject, Action<T>? valueChanged = null)
    {
        return Get(GetEnvironment(), GetCluster(), GetAppId(), configObject, valueChanged);
    }

    public T Get<T>(string environment, string cluster, string appId, string configObject, Action<T>? valueChanged = null)
    {
        var key = FormatKey(environment, cluster, appId, configObject);

        var value =  _taskJsonObjectsBySync.GetOrAdd(key, k => new Lazy<object>(() =>
        {
            var result =  GetRawByKey(k, (value) =>
            {
                var result = JsonSerializer.Deserialize<T>(value, _dynamicJsonSerializerOptions);

                var newValue = new Lazy<Task<object>>(() => Task.FromResult((object)result!));
                _taskJsonObjectsByAsync.AddOrUpdate(k, newValue, (_, _) => newValue);
                valueChanged?.Invoke(result!);
            }, GetSecret(appId));
            if (typeof(T).GetInterfaces().Any(type => type == typeof(IConvertible)))
            {
                if (result.ConfigurationType == ConfigurationTypes.Text)
                    return Convert.ChangeType(result.Raw, typeof(T));

                throw new FormatException(result.Raw);
            }

            return JsonSerializer.Deserialize<T>(result.Raw, _dynamicJsonSerializerOptions) ??
                throw new MasaException($"The content of [{configObject}] is wrong");
        })).Value;

        return (T)value;
    }

    public dynamic GetDynamic(string environment, string cluster, string appId, string configObject, Action<dynamic>? valueChanged = null)
    {
        var key = FormatKey(environment, cluster, appId, configObject);

        return GetDynamic(key, (k, value, options) =>
        {
            var result = JsonSerializer.Deserialize<ExpandoObject>(value, options);
            var newValue = new Lazy<ExpandoObject?>(() => result!);
            _taskExpandoObjectsBySync.AddOrUpdate(k, newValue!, (_, _) => newValue!);
            valueChanged?.Invoke(result!);
        }, GetSecret(appId));
    }

    public dynamic GetDynamic(string configObject)
    {
        var appId = GetAppId();
        var key = FormatKey(GetEnvironment(), GetCluster(), appId, configObject);
        return GetDynamic(key, null, GetSecret(appId));
    }

    internal dynamic GetDynamic(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged, string secret)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return GetDynamicInternal(key, valueChanged, secret);
    }

    private  dynamic GetDynamicInternal(
        string key,
        Action<string, dynamic, JsonSerializerOptions>? valueChanged,
        string secret)
    {
        var value = _taskExpandoObjectsBySync.GetOrAdd(key, k => new Lazy<ExpandoObject>( () =>
        {
            var raw =  GetRawByKey(k, value =>
            {
                valueChanged?.Invoke(k, value, _dynamicJsonSerializerOptions);
            }, secret);
            return JsonSerializer.Deserialize<ExpandoObject>(raw.Raw, _dynamicJsonSerializerOptions) ?? throw new ArgumentException(key);
        })).Value;

        return  value;
    }

    internal (string Raw, ConfigurationTypes ConfigurationType) GetRawByKey(
        string key,
        Action<string>? valueChanged,
        string secret)
    {
        var publishRelease =  _multilevelCacheClient.Get<PublishReleaseModel>(key, value =>
        {
            var result = FormatRaw(value, key, secret);
            valueChanged?.Invoke(result.Raw);
        });

        return FormatRaw(publishRelease, key, secret);
    }

    #endregion

    protected virtual (string Raw, ConfigurationTypes ConfigurationType) FormatRaw(
        PublishReleaseModel? publishRelease,
        string key,
        string secret)
    {
        var result = FormatPublishRelease(publishRelease, key, secret);

        switch (result.ConfigFormat)
        {
            case ConfigFormats.JSON:
                return (result.Content!, ConfigurationTypes.Json);

            case ConfigFormats.RAW:
                return (result.Content!, ConfigurationTypes.Text);

            case ConfigFormats.Properties:
                try
                {
                    var properties = PropertyConfigurationParser.Parse(result.Content!, _jsonSerializerOptions);
                    return (JsonSerializer.Serialize(properties, _jsonSerializerOptions), ConfigurationTypes.Properties);
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception,
                        "Dcc.ConfigurationApiClient: configObject invalid, {ParamName} is not a valid Properties type", key);
                    throw new ArgumentException("configObject invalid");
                }

            case ConfigFormats.XML:
                try
                {
                    var json = XmlConfigurationParser.XmlToJson(result.Content!);
                    return (json, ConfigurationTypes.Xml);
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception, "Dcc.ConfigurationApiClient: configObject invalid, {ParamName} is not a valid Xml type",
                        key);
                    throw new ArgumentException("configObject invalid");
                }

            case ConfigFormats.YAML:
                try
                {
                    var yamlObject = _yamlDeserializer.Deserialize<object>(result.Content!);

                    var json = _yamlSerializer.Serialize(yamlObject);
                    return (json, ConfigurationTypes.Yaml);
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception, "Dcc.ConfigurationApiClient: configObject invalid, {ParamName} is not a valid Yaml type",
                        key);
                    throw new ArgumentException("configObject invalid");
                }

            default:
                throw new NotSupportedException("Unsupported configuration type");
        }
    }

    private string FormatKey(string environment, string cluster, string appId, string configObject)
        => $"{GetEnvironment(environment)}-{GetCluster(cluster)}-{GetAppId(appId)}-{GetConfigObject(configObject)}".ToLower();

    private PublishReleaseModel FormatPublishRelease(PublishReleaseModel? publishRelease, string key, string secret)
    {
        if (publishRelease == null)
            throw new ArgumentException($"configObject invalid, {key} is not null");

        if (publishRelease.ConfigFormat == 0)
            throw new ArgumentException($"Dcc.ConfigurationApiClient: configObject invalid, {key} is an unsupported type");

        if (!publishRelease.Encryption)
            return publishRelease;

        MasaArgumentException.ThrowIfNullOrWhiteSpace(secret);
        publishRelease.Content = DecryptContent(secret, publishRelease.Content);

        return publishRelease;
    }

    private static string? DecryptContent(string secret, string? content)
    {
        if (string.IsNullOrEmpty(content) || content == "{}" || content == "[]")
            return content;

        var encryptContent = AesUtils.Decrypt(content, secret, FillType.Left);
        return encryptContent;
    }
}
