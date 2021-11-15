namespace MASA.Contrib.BasicAbility.Dcc;

public class ConfigurationAPIClient : ConfigurationAPIBase, IConfigurationAPIClient
{
    private readonly IMemoryCacheClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private readonly ConcurrentDictionary<string, Lazy<Task<ExpandoObject>>> _taskExpandoObjects = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _taskJsonObjects = new();

    public ConfigurationAPIClient(
        IMemoryCacheClient client,
        JsonSerializerOptions jsonSerializerOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(defaultSectionOption, expandSectionOptions)
    {
        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string environment, string cluster, string appId, string configObject, Action<string> valueChanged)
    {
        var key = FomartKey(environment, cluster, appId, configObject);
        return GetRawByKeyAsync(key, valueChanged);
    }

    public async Task<T> GetAsync<T>(string environment, string cluster, string appId, string configObject, Action<T> valueChanged)
        where T : class
    {
        var key = FomartKey(environment, cluster, appId, configObject);

        var value = await _taskJsonObjects.GetOrAdd(key, (k) => new Lazy<Task<object>>(async () =>
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

            return JsonSerializer.Deserialize<T>(result.Raw, options) ?? throw new ArgumentException(nameof(configObject));

        })).Value;

        return (value as T)!;
    }

    public async Task<dynamic> GetDynamicAsync(string environment, string cluster, string appId, string configObject, Action<dynamic> valueChanged)
    {
        var key = FomartKey(environment, cluster, appId, configObject);

        var value = _taskExpandoObjects.GetOrAdd(key, (k) => new Lazy<Task<ExpandoObject>>(async () =>
        {
            var options = new JsonSerializerOptions(_jsonSerializerOptions);
            options.EnableDynamicTypes();

            var raw = await GetRawByKeyAsync(k, (value) =>
            {
                var result = JsonSerializer.Deserialize<ExpandoObject>(value, options);
                var newValue = new Lazy<Task<ExpandoObject?>>(() => Task.FromResult(result)!);
                _taskExpandoObjects.AddOrUpdate(k, newValue!, (_, _) => newValue!);
                valueChanged?.Invoke(result!);
            });

            return JsonSerializer.Deserialize<ExpandoObject>(raw.Raw, options) ?? throw new ArgumentException(nameof(configObject));
        })).Value;

        return await value;
    }

    private async Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsync(string key, Action<string> valueChanged)
    {
        var raw = await _client.GetAsync<string>(key, (value) =>
        {
            var result = FormatRaw(value);
            valueChanged?.Invoke(result.Raw);
        });

        return FormatRaw(raw);
    }

    private (string Raw, ConfigurationTypes ConfigurationType) FormatRaw(string raw)
    {
        if (raw == null)
            throw new ArgumentException("configObject invalid");

        var result = JsonSerializer.Deserialize<PublishRelease>(raw, _jsonSerializerOptions);
        if (result == null || result.ConfigFormat == 0)
            throw new ArgumentException("configObject invalid");

        switch (result.ConfigFormat)
        {
            case ConfigFormats.Json:
                return (result.Content!, ConfigurationTypes.Json);

            case ConfigFormats.Text:
                return (result.Content!, ConfigurationTypes.Text);

            case ConfigFormats.Properties:
                var properties = PropertyConfigurationParser.Parse(result.Content!, _jsonSerializerOptions);
                if (properties == null)
                    throw new ArgumentException("configObject invalid");

                return (JsonSerializer.Serialize(properties, _jsonSerializerOptions), ConfigurationTypes.Properties);

            default:
                throw new NotSupportedException("Unsupported configuration type");
        }
    }

    private string FomartKey(string environment, string cluster, string appId, string configObject)
        => $"{GetEnvironment(environment)}-{GetCluster(cluster)}-{GetAppid(appId)}-{GetConfigObject(configObject)}".ToLower();
}
