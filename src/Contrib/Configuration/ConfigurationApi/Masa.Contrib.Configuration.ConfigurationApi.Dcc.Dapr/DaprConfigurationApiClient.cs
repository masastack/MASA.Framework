// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr;

public class DaprConfigurationApiClient : ConfigurationApiClientBase
{
    private readonly DaprClient _client;
    private readonly ILogger<DaprConfigurationApiClient>? _logger;
    private readonly string _storeName;
    private readonly ConcurrentDictionary<string, byte> _subscribedKeys = new();

    public DaprConfigurationApiClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        DccDaprOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(
            defaultSectionOption,
            expandSectionOptions,
            jsonSerializerOptions,
            defaultSectionOption.ConfigObjectSecret,
            serviceProvider.GetService<ILogger<DaprConfigurationApiClient>>())
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultSectionOption.StoreName);
#else
        if (string.IsNullOrWhiteSpace(defaultSectionOption.StoreName))
            throw new ArgumentException("StoreName cannot be null or whitespace.", nameof(defaultSectionOption));
#endif

        var client = serviceProvider.GetRequiredService<DaprClient>();
        ArgumentNullException.ThrowIfNull(client);

        _storeName = defaultSectionOption.StoreName;
        _client = client;
        _logger = serviceProvider.GetService<ILogger<DaprConfigurationApiClient>>();
    }

    protected override async Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsync(string key,
        Action<string>? valueChanged)
    {
        var response = await GetConfigurationAsync(key).ConfigureAwait(false);
        PublishReleaseModel? model = null;
        if (response.Items != null && response.Items.TryGetValue(key, out var configItem))
        {
            model = JsonSerializer.Deserialize<PublishReleaseModel>(configItem.Value, JsonSerializerOptions);
        }

        var result = FormatRaw(model, key);
        EnsureSubscribe(key, valueChanged);
        return result;
    }

    protected override string FomartKey(string environment, string cluster, string appId, string configObject)
        => $"{DEFAULT_PREFIX}{GetEnvironment(environment)}-{GetCluster(cluster)}-{GetAppId(appId)}-{GetConfigObject(configObject)}".ToLowerInvariant();

    private void EnsureSubscribe(string key, Action<string>? valueChanged)
    {
        if (valueChanged == null || !_subscribedKeys.TryAdd(key, 0))
            return;

        _ = Task.Run(async () =>
        {
            try
            {
                using var cts = new CancellationTokenSource();
#pragma warning disable CS0618
                var subscribeResponse = await _client
                    .SubscribeConfiguration(_storeName, new[] { key }, cancellationToken: cts.Token)
                    .ConfigureAwait(false);
#pragma warning restore CS0618

                await foreach (var configurationItems in subscribeResponse.Source.WithCancellation(cts.Token).ConfigureAwait(false))
                {
                    if (configurationItems == null)
                        continue;

                    foreach (var (configKey, configItem) in configurationItems)
                    {
                        if (!string.Equals(configKey, key, StringComparison.OrdinalIgnoreCase))
                            continue;

                        var model = JsonSerializer.Deserialize<PublishReleaseModel>(configItem.Value, JsonSerializerOptions);
                        var result = FormatRaw(model, key);
                        valueChanged.Invoke(result.Raw);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Dapr configuration subscribe failed for key {Key}", key);
                _subscribedKeys.TryRemove(key, out _);
            }
        });
    }

#pragma warning disable CS0618
    private Task<GetConfigurationResponse> GetConfigurationAsync(string key)
        => _client.GetConfiguration(_storeName, new[] { key });
#pragma warning restore CS0618
}
