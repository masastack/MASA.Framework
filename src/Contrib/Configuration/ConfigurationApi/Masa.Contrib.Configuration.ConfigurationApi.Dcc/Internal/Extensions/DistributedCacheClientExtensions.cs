// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Extensions;


internal static class DistributedCacheClientExtensions
{
    public static List<string> GetAllConfigObjects(
        this IDistributedCacheClient distributedCacheClient,
        string appId,
        string environment,
        string cluster)
    {
        var defaultConfigObjects = new List<string>();

        string partialKey =
            $"{environment}-{cluster}-{appId}".ToLower();
        var keys = distributedCacheClient.GetKeys<PublishReleaseModel>($"{partialKey}*");
        foreach (var key in keys)
        {
            var configObject = key.Split($"{partialKey}-", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (configObject == null) continue;

            defaultConfigObjects.Add(configObject);
        }

        return defaultConfigObjects;
    }
}
