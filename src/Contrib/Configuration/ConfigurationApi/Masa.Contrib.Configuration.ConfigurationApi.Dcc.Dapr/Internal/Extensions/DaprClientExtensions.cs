// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Internal.Extensions;

internal static class DaprClientExtensions
{
    public static List<string> GetAllConfigObjects(
        this DaprClient client,
        string storeName,
        string appId,
        string environment,
        string cluster,
        string prefix)
    {
        var configObjects = new List<string>();
        var partialKey = $"{prefix}{environment}-{cluster}-{appId}-".ToLowerInvariant();
#pragma warning disable CS0618
        var response = client.GetConfiguration(storeName, null).ConfigureAwait(false).GetAwaiter().GetResult();
#pragma warning restore CS0618
        foreach (var key in response.Items.Keys)
        {
            if (!key.StartsWith(partialKey, StringComparison.OrdinalIgnoreCase))
                continue;

            var configObject = key[partialKey.Length..];
            if (!string.IsNullOrEmpty(configObject))
                configObjects.Add(configObject);
        }

        return configObjects;
    }
}
