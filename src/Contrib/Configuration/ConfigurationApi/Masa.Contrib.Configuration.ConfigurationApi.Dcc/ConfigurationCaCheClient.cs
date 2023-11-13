// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

public class ConfigurationCaCheClient : ConfigurationApiBase, IConfigurationApiClient
{
    private readonly IMultilevelCacheClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JsonSerializerOptions _dynamicJsonSerializerOptions;
    private readonly ILogger<ConfigurationApiClient>? _logger;
    private readonly DccOptions _dccOptions;

    private readonly ConcurrentDictionary<string, Lazy<Task<ExpandoObject>>> _taskExpandoObjects = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _taskJsonObjects = new();
    private readonly Masa.BuildingBlocks.Data.ISerializer _yamlSerializer;
    private readonly Masa.BuildingBlocks.Data.IDeserializer _yamlDeserializer;

    public ConfigurationCaCheClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        DccOptions dccOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(defaultSectionOption, expandSectionOptions)
    {
        var client = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);
        ArgumentNullException.ThrowIfNull(client);

        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
        _dynamicJsonSerializerOptions = new JsonSerializerOptions(_jsonSerializerOptions);
        _dynamicJsonSerializerOptions.EnableDynamicTypes();
        _logger = serviceProvider.GetService<ILogger<ConfigurationApiClient>>();
        _yamlSerializer = new DefaultYamlSerializer(new SerializerBuilder().JsonCompatible().Build());
        _yamlDeserializer = new DefaultYamlDeserializer(new DeserializerBuilder().Build());
        _dccOptions = dccOptions;
    }

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string configObject, Action<string>? valueChanged)
    {
        return GetRawAsync(GetEnvironment(string.Empty), GetCluster(string.Empty), GetAppId(string.Empty), configObject, valueChanged);
    }

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string environment, string cluster, string appId,
        string configObject, Action<string>? valueChanged)
    {
        var key = FomartKey(environment, cluster, appId, configObject);
        return GetRawByKeyAsync(key, valueChanged);
    }

    public Task<T> GetAsync<T>(string configObject, Action<T>? valueChanged)
    {
        return GetAsync(GetEnvironment(string.Empty), GetCluster(string.Empty), GetAppId(string.Empty), configObject, valueChanged);
    }

    public async Task<T> GetAsync<T>(string environment, string cluster, string appId, string configObject, Action<T>? valueChanged = null)
    {
        var key = FomartKey(environment, cluster, appId, configObject);

        var value = await _taskJsonObjects.GetOrAdd(key, k => new Lazy<Task<object>>(async () =>
        {
            var result = await GetRawByKeyAsync(k, (value) =>
            {
                var result = JsonSerializer.Deserialize<T>(value, _dynamicJsonSerializerOptions);

                var newValue = new Lazy<Task<object>>(() => Task.FromResult((object)result!));
                _taskJsonObjects.AddOrUpdate(k, newValue, (_, _) => newValue);
                valueChanged?.Invoke(result!);
            }).ConfigureAwait(false);
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

    public Task<dynamic> GetDynamicAsync(string environment, string cluster, string appId, string configObject,
        Action<dynamic>? valueChanged = null)
    {
        var key = FomartKey(environment, cluster, appId, configObject);

        return GetDynamicAsync(key, (k, value, options) =>
        {
            var result = JsonSerializer.Deserialize<ExpandoObject>(value, options);
            var newValue = new Lazy<Task<ExpandoObject?>>(() => Task.FromResult(result)!);
            _taskExpandoObjects.AddOrUpdate(k, newValue!, (_, _) => newValue!);
            valueChanged?.Invoke(result!);
        });
    }

    public Task<dynamic> GetDynamicAsync(string configObject)
    {
        var key = FomartKey(GetEnvironment(string.Empty), GetCluster(string.Empty), GetAppId(string.Empty), configObject);
        return GetDynamicAsync(key, null);
    }

    protected virtual Task<dynamic> GetDynamicAsync(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return GetDynamicInternalAsync(key, valueChanged);
    }

    private async Task<dynamic> GetDynamicInternalAsync(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged)
    {
        var value = _taskExpandoObjects.GetOrAdd(key, k => new Lazy<Task<ExpandoObject>>(async () =>
        {
            var raw = await GetRawByKeyAsync(k, value =>
            {
                valueChanged?.Invoke(k, value, _dynamicJsonSerializerOptions);
            }).ConfigureAwait(false);
            return JsonSerializer.Deserialize<ExpandoObject>(raw.Raw, _dynamicJsonSerializerOptions) ?? throw new ArgumentException(key);
        })).Value.ConfigureAwait(false);

        return await value;
    }

    protected virtual async Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsync(string key,
        Action<string>? valueChanged)
    {
        var publishRelease = await _client.GetAsync<PublishReleaseModel>(key, value =>
        {
            var result = FormatRaw(value, key);
            valueChanged?.Invoke(result.Raw);
        }).ConfigureAwait(false);

        return FormatRaw(publishRelease, key);
    }

    protected virtual (string Raw, ConfigurationTypes ConfigurationType) FormatRaw(PublishReleaseModel? publishRelease, string key)
    {
        PublishReleaseModel result = FormatPublishRelease(publishRelease, key);

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

    private string FomartKey(string environment, string cluster, string appId, string configObject)
        => $"{GetEnvironment(environment)}-{GetCluster(cluster)}-{GetAppId(appId)}-{GetConfigObject(configObject)}".ToLower();

    private PublishReleaseModel FormatPublishRelease(PublishReleaseModel? publishRelease, string key)
    {
        if (publishRelease == null)
            throw new ArgumentException($"configObject invalid, {key} is not null");

        if (publishRelease.ConfigFormat == 0)
            throw new ArgumentException($"Dcc.ConfigurationApiClient: configObject invalid, {key} is an unsupported type");

        if (publishRelease.Encryption)
        {
            if (string.IsNullOrEmpty(_dccOptions.ConfigObjectSecret))
            {
                throw new ArgumentNullException(_dccOptions.ConfigObjectSecret, nameof(_dccOptions.ConfigObjectSecret));
            }
            publishRelease.Content = DecryptContent(_dccOptions.ConfigObjectSecret, publishRelease.Content);
        }

        return publishRelease;
    }

    private static string? DecryptContent(string secret, string? content)
    {
        if (!string.IsNullOrEmpty(content) && content != "{}" && content != "[]")
        {
            var encryptContent = AesUtils.Decrypt(content, secret, FillType.Left);
            return encryptContent;
        }
        else
        {
            return content;
        }
    }

    Task<List<(string ConfigObject, string Raw, ConfigurationTypes ConfigurationType)>> IConfigurationApiClient.GetRawsAsync(string environment, string cluster, string appId, params string[] configObjects)
    {
        throw new NotImplementedException();
    }

    Task<List<(string ConfigObject, string Raw, ConfigurationTypes ConfigurationType)>> IConfigurationApiClient.GetRawsAsync(string environment, string cluster, string appId, Action<List<string>>? valueChanged, params string[] configObjects)
    {
        throw new NotImplementedException();
    }
}
