// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

public class ConfigurationApiClient : ConfigurationApiBase, IConfigurationApiClient
{
    private readonly IMemoryCacheClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<ConfigurationApiClient>? _logger;
    private readonly DccOptions _dccOptions;

    private readonly ConcurrentDictionary<string, Lazy<Task<ExpandoObject>>> _taskExpandoObjects = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _taskJsonObjects = new();
    private readonly Masa.BuildingBlocks.Data.ISerializer _yamlSerializer;
    private readonly Masa.BuildingBlocks.Data.IDeserializer _yamlDeserializer;

    public ConfigurationApiClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        DccOptions dccOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(defaultSectionOption, expandSectionOptions)
    {
        var client = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);
        ArgumentNullException.ThrowIfNull(client);

        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
        _logger = serviceProvider.GetService<ILogger<ConfigurationApiClient>>();
        _yamlSerializer = serviceProvider.GetRequiredService<ISerializerFactory>().Create(DEFAULT_CLIENT_NAME);
        _yamlDeserializer = serviceProvider.GetRequiredService<IDeserializerFactory>().Create(DEFAULT_CLIENT_NAME);
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

    public async Task<T> GetAsync<T>(string environment, string cluster, string appId, string configObject, Action<T>? valueChanged)
    {
        var key = FomartKey(environment, cluster, appId, configObject);

        var value = await _taskJsonObjects.GetOrAdd(key, k => new Lazy<Task<object>>(async () =>
        {
            var options = new JsonSerializerOptions(_jsonSerializerOptions);
            options.EnableDynamicTypes();

            var result = await GetRawByKeyAsync(k, (value) =>
            {
                var result = JsonSerializer.Deserialize<T>(value, options);

                var newValue = new Lazy<Task<object>>(() => Task.FromResult((object)result!));
                _taskJsonObjects.AddOrUpdate(k, newValue, (_, _) => newValue);
                valueChanged?.Invoke(result!);
            });
            if (typeof(T).GetInterfaces().Any(type => type == typeof(IConvertible)))
            {
                if (result.ConfigurationType == ConfigurationTypes.Text)
                    return Convert.ChangeType(result.Raw, typeof(T));

                throw new FormatException(result.Raw);
            }

            return JsonSerializer.Deserialize<T>(result.Raw, options) ?? throw new ArgumentException(nameof(configObject));
        })).Value;

        return (T)value;
    }

    public async Task<dynamic> GetDynamicAsync(string environment, string cluster, string appId, string configObject,
        Action<dynamic>? valueChanged = null)
    {
        var key = FomartKey(environment, cluster, appId, configObject);

        return await GetDynamicAsync(key, (k, value, options) =>
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

    protected virtual async Task<dynamic> GetDynamicAsync(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return await GetDynamicInternalAsync(key, valueChanged);
    }

    private async Task<dynamic> GetDynamicInternalAsync(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged)
    {
        var value = _taskExpandoObjects.GetOrAdd(key, k => new Lazy<Task<ExpandoObject>>(async () =>
        {
            var options = new JsonSerializerOptions(_jsonSerializerOptions);
            options.EnableDynamicTypes();

            var raw = await GetRawByKeyAsync(k, value =>
            {
                valueChanged?.Invoke(k, value, options);
            });
            return JsonSerializer.Deserialize<ExpandoObject>(raw.Raw, options) ?? throw new ArgumentException(key);
        })).Value;

        return await value;
    }

    protected virtual async Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsync(string key,
        Action<string>? valueChanged)
    {
        var raw = await _client.GetAsync<string>(key, value =>
        {
            var result = FormatRaw(value, key);
            valueChanged?.Invoke(result.Raw);
        });

        return FormatRaw(raw, key);
    }

    protected virtual (string Raw, ConfigurationTypes ConfigurationType) FormatRaw(string? raw, string paramName)
    {
        PublishRelease result = GetPublishRelease(raw, paramName);

        switch (result.ConfigFormat)
        {
            case ConfigFormats.Json:
                return (result.Content!, ConfigurationTypes.Json);

            case ConfigFormats.Raw:
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
                        "Dcc.ConfigurationApiClient: configObject invalid, {ParamName} is not a valid Properties type", paramName);
                    throw new ArgumentException("configObject invalid");
                }

            case ConfigFormats.Xml:
                try
                {
                    var json = XmlConfigurationParser.XmlToJson(result.Content!);
                    return (json, ConfigurationTypes.Xml);
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception, "Dcc.ConfigurationApiClient: configObject invalid, {ParamName} is not a valid Xml type",
                        paramName);
                    throw new ArgumentException("configObject invalid");
                }

            case ConfigFormats.Yaml:
                try
                {
                    var yamlObject = _yamlDeserializer.Deserialize<object>(result.Content!);

                    var json = _yamlSerializer.Serialize(yamlObject);
                    return (json, ConfigurationTypes.Yaml);
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception, "Dcc.ConfigurationApiClient: configObject invalid, {ParamName} is not a valid Yaml type",
                        paramName);
                    throw new ArgumentException("configObject invalid");
                }

            default:
                throw new NotSupportedException("Unsupported configuration type");
        }
    }

    private string FomartKey(string environment, string cluster, string appId, string configObject)
        => $"{GetEnvironment(environment)}-{GetCluster(cluster)}-{GetAppId(appId)}-{GetConfigObject(configObject)}".ToLower();

    private PublishRelease GetPublishRelease(string? raw, string paramName)
    {
        if (raw == null)
            throw new ArgumentException($"configObject invalid, {paramName} is not null");

        PublishRelease? result;
        try
        {
            result = JsonSerializer.Deserialize<PublishRelease>(raw, _jsonSerializerOptions);
        }
        catch (Exception exception)
        {
            _logger?.LogWarning(exception, "Dcc.ConfigurationApiClient: configObject invalid, {ParamName} is not a valid response value",
                paramName);
            throw new ArgumentException($"Dcc.ConfigurationApiClient: configObject invalid, {paramName} is not a valid response value");
        }
        if (result == null || result.ConfigFormat == 0)
            throw new ArgumentException($"Dcc.ConfigurationApiClient: configObject invalid, {paramName} is an unsupported type");

        if (result.Encryption)
        {
            if (string.IsNullOrEmpty(_dccOptions.ConfigObjectSecret))
            {
                throw new ArgumentNullException(_dccOptions.ConfigObjectSecret, nameof(_dccOptions.ConfigObjectSecret));
            }
            result.Content = DecryptContent(_dccOptions.ConfigObjectSecret, result.Content);
        }

        return result;
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
}
