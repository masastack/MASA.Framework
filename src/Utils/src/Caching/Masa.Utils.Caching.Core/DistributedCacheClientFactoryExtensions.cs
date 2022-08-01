// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core;

/// <summary>
/// Extension methods for <see cref="IDistributedCacheClientFactory"/>.
/// </summary>
public static class DistributedCacheClientFactoryExtensions
{
    /// <summary>
    /// Creates a new <see cref="IDistributedCacheClient"/> using the default configuration.
    /// </summary>
    /// <param name="factory">The <see cref="IDistributedCacheClientFactory"/>.</param>
    /// <returns>An <see cref="IDistributedCacheClient"/> configured using the default configuration.</returns>
    public static IDistributedCacheClient CreateClient(this IDistributedCacheClientFactory factory)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        return factory.CreateClient(string.Empty);
    }
}
