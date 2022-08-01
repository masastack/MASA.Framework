// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.DistributedMemory;

/// <summary>
/// Extension methods for <see cref="IMemoryCacheClientFactory"/>.
/// </summary>
public static class MemoryCacheClientFactoryExtensions
{
    /// <summary>
    /// Creates a new <see cref="IMemoryCacheClient"/> using the default configuration.
    /// </summary>
    /// <param name="factory">The <see cref="IMemoryCacheClientFactory"/>.</param>
    /// <returns>An <see cref="IMemoryCacheClient"/> configured using the default configuration.</returns>
    public static IMemoryCacheClient CreateClient(this IMemoryCacheClientFactory factory)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        return factory.CreateClient(string.Empty);
    }
}
