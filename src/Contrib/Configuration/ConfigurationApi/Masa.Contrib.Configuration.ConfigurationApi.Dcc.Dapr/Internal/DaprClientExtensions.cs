// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Redis.Internal.Extensions;

internal static class DaprClientExtensions
{
    public static List<string> GetAllConfigObjects(
        this DaprClient client,
        string storeName,
        string appId,
        string environment,
        string cluster)
    {
        var defaultConfigObjects = new List<string>();

        string partialKey = $"{environment}-{cluster}-{appId}-".ToLower();
        var response = client.GetConfiguration(storeName, null).ConfigureAwait(false).GetAwaiter().GetResult();
        foreach (var key in response.Items.Keys)
        {
            if (!key.StartsWith(partialKey)) continue;
            var configObject = key.Split(partialKey, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (configObject == null) continue;
            defaultConfigObjects.Add(configObject);
        }

        return defaultConfigObjects;
    }
}
