// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core.DependencyInjection;

/// <summary>
/// Extension methods for configuring an <see cref="ICachingBuilder"/>
/// </summary>
public static class DistributedCacheClientBuilderExtensions
{
    /// <summary>
    /// Adds a delegate that will be used to configure a named <see cref="IDistributedCacheClient"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ICachingBuilder"/>.</param>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="IDistributedCacheClient"/>.</param>
    /// <returns>An <see cref="ICachingBuilder"/> that can be used to configure the client.</returns>
    public static ICachingBuilder ConfigureDistributedCacheClient<TOptions>(this ICachingBuilder builder, Action<TOptions> configureOptions)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));

        builder.Services.Configure(builder.Name, configureOptions);
        return builder;
    }

    /// <summary>
    /// Adds a delegate that will be used to configure a named <see cref="IDistributedCacheClient"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ICachingBuilder"/>.</param>
    /// <param name="configuration"></param>
    /// <returns>An <see cref="ICachingBuilder"/> that can be used to configure the client.</returns>
    public static ICachingBuilder ConfigureDistributedCacheClient<TOptions>(this ICachingBuilder builder, IConfiguration configuration)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        builder.Services.Configure<TOptions>(builder.Name, configuration);
        return builder;
    }
}
