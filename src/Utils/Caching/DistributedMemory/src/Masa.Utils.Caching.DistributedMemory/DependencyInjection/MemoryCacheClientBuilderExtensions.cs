// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.DistributedMemory.DependencyInjection;

/// <summary>
/// Extension methods for configuring an <see cref="ICachingBuilder"/>
/// </summary>
public static class MemoryCacheClientBuilderExtensions
{
    /// <summary>
    /// Adds a delegate that will be used to configure a named <see cref="IMemoryCacheClient"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ICachingBuilder"/>.</param>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="IDistributedCacheClient"/>.</param>
    /// <returns>An <see cref="ICachingBuilder"/> that can be used to configure the client.</returns>
    public static ICachingBuilder ConfigureMemoryCacheClient(this ICachingBuilder builder, Action<MasaMemoryCacheOptions> configureOptions)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (configureOptions == null)
        {
            throw new ArgumentNullException(nameof(configureOptions));
        }

        builder.Services.Configure(builder.Name, configureOptions);

        return builder;
    }
}
