// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.DistributedMemory.DependencyInjection;

/// <summary>
/// Extension methods to configure an <see cref="ICachingBuilder"/> for <see cref="IMemoryCacheClientFactory"/>.
/// </summary>
public static class MemoryCacheClientFactoryCachingBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="IMemoryCacheClientFactory"/> and related services to the <see cref="ICachingBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ICachingBuilder"/>.</param>
    /// <returns>The <see cref="ICachingBuilder"/>.</returns>
    public static ICachingBuilder AddMasaMemoryCache(this ICachingBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var name = builder.Name;

        builder.Services.TryAddSingleton<IMemoryCacheClientFactory, MemoryCacheClientFactory>();

        builder.Services.TryAddSingleton<IMemoryCacheClient>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>();

            return factory.CreateClient(name);
        });

        return builder;
    }

    /// <summary>
    ///  Adds the <see cref="IMemoryCacheClientFactory"/> and related services to the <see cref="ICachingBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ICachingBuilder"/>.</param>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="MemoryCacheClient"/>.</param>
    /// <returns>The <see cref="ICachingBuilder"/>.</returns>
    public static ICachingBuilder AddMasaMemoryCache(this ICachingBuilder builder, Action<MasaMemoryCacheOptions> configureOptions)
    {
        if (configureOptions == null)
        {
            throw new ArgumentNullException(nameof(configureOptions));
        }

        builder.AddMasaMemoryCache();

        builder.ConfigureMemoryCacheClient(configureOptions);

        return builder;
    }
}
