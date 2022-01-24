namespace MASA.Contrib.BasicAbility.Dcc;

public class ConfigurationAPIClient : ConfigurationAPIBase, IConfigurationAPIClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCacheClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private readonly ConcurrentDictionary<string, Lazy<Task<ExpandoObject>>> _taskExpandoObjects = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _taskJsonObjects = new();

    public ConfigurationAPIClient(
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
        Action<dynamic> valueChanged)
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

    public async Task<dynamic> GetDynamicAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        key = key.Replace(".", ConfigurationPath.KeyDelimiter);
        return await Task.FromResult(Format(configuration.GetSection(key)));
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

    private (string Raw, ConfigurationTypes ConfigurationType) FormatRaw(string? raw)
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
        => $"{GetEnvironment(environment)}-{GetCluster(cluster)}-{GetAppId(appId)}-{GetConfigObject(configObject)}".ToLower();

    private dynamic Format(IConfigurationSection section)
    {
        var childrenSections = section.GetChildren();
        if (!section.Exists() || !childrenSections.Any())
        {
            return section.Value;
        }
        else
        {
            var result = new ExpandoObject();
            var parent = result as IDictionary<string, object>;
            foreach (var child in childrenSections)
            {
                parent[child.Key] = Format(child);
            }
            return result;
        }
    }
}
