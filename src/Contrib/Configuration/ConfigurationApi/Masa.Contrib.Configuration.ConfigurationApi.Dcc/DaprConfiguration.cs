// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;
public class DaprConfiguration
{
    private Action<string> _valueChanged;
    private string _key;
    private readonly DaprClient _client;

    public DaprConfiguration(DaprClient daprClient)
    {
        _client = daprClient;
    }

    public async Task<string> GetValueAsync(string key, Action<string>? valueChanged = null)
    {
        _key = key;
        GetConfigurationResponse configuration = await _client.GetConfiguration(
            CONFIGURATION_API_STORE_NAME,
            new List<string> { key })
            .ConfigureAwait(false);

        if (valueChanged != null)
        {
            _valueChanged = valueChanged;
            var timer = new System.Timers.Timer
            {
                Interval = 1,
                AutoReset = false
            };
            timer.Elapsed += async (o, e) => await Task.Run(async () => await Subscribe());
            timer.Start();
        }

        if (!configuration.Items.Any())
        {
            throw new MasaException($"The key: \"{key}\" does not have a value");
        }

        return configuration.Items.First().Value.Value;
    }

    public async Task Subscribe()
    {
        try
        {
            SubscribeConfigurationResponse subscribe = await _client.SubscribeConfiguration(
                DATA_DICTIONARY_SECTION_NAME,
                new List<string> { _key })
            .ConfigureAwait(false);

            if (subscribe != null)
            {
                await foreach (var items in subscribe.Source)
                {
                    if (items.Keys.Count != 0)
                    {
                        bool b = items.TryGetValue(_key, out ConfigurationItem? configurationItem);
                        if (b)
                            _valueChanged.Invoke(configurationItem!.Value);
                    }
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
