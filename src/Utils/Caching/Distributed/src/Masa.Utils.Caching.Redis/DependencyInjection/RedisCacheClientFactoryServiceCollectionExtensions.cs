// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods to configure an <see cref="IServiceCollection"/> for <see cref="IDistributedCacheClientFactory"/>.
/// </summary>
public static class RedisCacheClientFactoryServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IDistributedCacheClientFactory"/> and related services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="IDistributedCacheClient"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static ICachingBuilder AddMasaRedisCache(this IServiceCollection services, Action<RedisConfigurationOptions> configureOptions)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
            return factory.CreateClient(string.Empty);
        });

        return services.AddMasaRedisCache(string.Empty, configureOptions);
    }

    /// <summary>
    /// Adds the <see cref="IDistributedCacheClientFactory"/> and related services to the <see cref="IServiceCollection"/> and configures a named <see cref="IDistributedCacheClient"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="name">The logical name of the <see cref="IDistributedCacheClient"/> to configure.</param>
    /// <param name="configureOptions">A delegate that is used to configure an <see cref="IDistributedCacheClient"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static ICachingBuilder AddMasaRedisCache(
        this IServiceCollection services,
        string name,
        Action<RedisConfigurationOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));

        services.TryAddSingleton<IDistributedCacheClientFactory, RedisCacheClientFactory>();

        var builder = new CachingBuilder(services, name);

        builder.ConfigureDistributedCacheClient(configureOptions);

        return builder;
    }

    /// <summary>
    /// Adds the <see cref="IDistributedCacheClientFactory"/> and related services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"></param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static ICachingBuilder AddMasaRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
            return factory.CreateClient(string.Empty);
        });

        return services.AddMasaRedisCache(string.Empty, configuration);
    }

    /// <summary>
    /// Adds the <see cref="IDistributedCacheClientFactory"/> and related services to the <see cref="IServiceCollection"/> and configures a named <see cref="IDistributedCacheClient"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="name">The logical name of the <see cref="IDistributedCacheClient"/> to configure.</param>
    /// <param name="configuration"></param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static ICachingBuilder AddMasaRedisCache(
        this IServiceCollection services,
        string name,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.TryAddSingleton<IDistributedCacheClientFactory, RedisCacheClientFactory>();

        var builder = new CachingBuilder(services, name);
        builder.ConfigureDistributedCacheClient<RedisConfigurationOptions>(configuration);
        return builder;
    }

    /// <summary>
    /// Adds the <see cref="IDistributedCacheClientFactory"/> and related services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="RedisConfigurationOptions"/> to configure an <see cref="IDistributedCacheClient"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static ICachingBuilder AddMasaRedisCache(this IServiceCollection services, RedisConfigurationOptions options)
        => services.AddMasaRedisCache(o => o.Initialize(options));

    /// <summary>
    /// Adds the <see cref="IDistributedCacheClientFactory"/> and related services to the <see cref="IServiceCollection"/> and configures a named <see cref="IDistributedCacheClient"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="name">The logical name of the <see cref="IDistributedCacheClient"/> to configure.</param>
    /// <param name="options">The <see cref="RedisConfigurationOptions"/> to configure an <see cref="IDistributedCacheClient"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static ICachingBuilder AddMasaRedisCache(this IServiceCollection services, string name, RedisConfigurationOptions options)
        => services.AddMasaRedisCache(name, o => o.Initialize(options));
}
