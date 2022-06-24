// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

public class ConfigurationApiClient : ConfigurationApiBase, IConfigurationApiClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCacheClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<ConfigurationApiClient>? _logger;

    private readonly ConcurrentDictionary<string, Lazy<Task<ExpandoObject>>> _taskExpandoObjects = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _taskJsonObjects = new();
    private readonly IDeserializer _deserializer = new DeserializerBuilder().Build();

    public ConfigurationApiClient(
        IServiceProvider serviceProvider,
        IMemoryCacheClient client,
        JsonSerializerOptions jsonSerializerOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(defaultSectionOption, expandSectionOptions)
    {
        _serviceProvider = serviceProvider;
        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
        _logger = serviceProvider.GetService<ILogger<ConfigurationApiClient>>();
    }

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string environment, string cluster, string appId,
        string configObject, Action<string> valueChanged)
    {
        var key = FomartKey(environment, cluster, appId, configObject);
        return GetRawByKeyAsync(key, valueChanged);
    }

    public async Task<T> GetAsync<T>(string environment, string cluster, string appId, string configObject, Action<T> valueChanged)
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

    public async Task<dynamic> GetDynamicAsync(string environment, string cluster, string appId, string configObject, Action<dynamic> valueChanged)
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

    public Task<dynamic> GetDynamicAsync(string key) => GetDynamicAsync(key, null);

    protected virtual async Task<dynamic> GetDynamicAsync(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

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

    protected virtual async Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsync(string key, Action<string>? valueChanged)
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
                        $"Dcc.ConfigurationApiClient: configObject invalid, {paramName} is not a valid Properties type");
                    throw new ArgumentException("configObject invalid");
                }

            case ConfigFormats.Xml:
                try
                {
                    var doc = XDocument.Parse(result.Content!);
                    var json = Newtonsoft.Json.JsonConvert.SerializeXNode(doc);
                    return (json, ConfigurationTypes.Xml);
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception,
                        $"Dcc.ConfigurationApiClient: configObject invalid, {paramName} is not a valid Xml type");
                    throw new ArgumentException("configObject invalid");
                }

            case ConfigFormats.Yaml:
                try
                {
                    var yamlObject = _deserializer.Deserialize<object>(result.Content!);

                    var serializer = new SerializerBuilder().JsonCompatible().Build();
                    var json = serializer.Serialize(yamlObject);
                    return (json, ConfigurationTypes.Yaml);
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception,
                        $"Dcc.ConfigurationApiClient: configObject invalid, {paramName} is not a valid Yaml type");
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
            string message = $"Dcc.ConfigurationApiClient: configObject invalid, {paramName} is not a valid response value";
            _logger?.LogWarning(exception, message);
            throw new ArgumentException(message);
        }
        if (result == null || result.ConfigFormat == 0)
            throw new ArgumentException($"Dcc.ConfigurationApiClient: configObject invalid, {paramName} is an unsupported type");

        return result;
    }
}
