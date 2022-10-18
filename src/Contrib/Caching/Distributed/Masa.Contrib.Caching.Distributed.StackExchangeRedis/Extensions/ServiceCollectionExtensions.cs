// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Deprecated method, will be removed in 1.0
/// </summary>
public static class ServiceCollectionExtensions
{
    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        Action<RedisConfigurationOptions> action)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        action.Invoke(redisConfigurationOptions);
        return services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            redisConfigurationOptions);
    }

    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(this IServiceCollection services,
        RedisConfigurationOptions redisConfigurationOptions)
        => services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            redisConfigurationOptions);

    /// <summary>
    /// Adds a default implementation for the <see cref="T:Masa.BuildingBlocks.Caching.IDistributedCacheClient" /> service.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(this IServiceCollection services)
        => services.AddStackExchangeRedisCache(Microsoft.Extensions.Options.Options.DefaultName);

    /// <summary>
    /// Add distributed Redis cache
    /// </summary>
    /// <param name="services"></param>
    /// <param name="name"></param>
    /// <param name="redisSectionName">redis node name, not required, default: RedisConfig(Use local configuration)</param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        string redisSectionName = Const.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddDistributedCacheCore(services, name);
        new DistributedCacheOptions(services, name).UseStackExchangeRedisCache(redisSectionName, jsonSerializerOptions);
        return new CachingBuilder(services, name);
    }

    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        Action<RedisConfigurationOptions> action,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        action.Invoke(redisConfigurationOptions);
        return services.AddStackExchangeRedisCache(
            name,
            redisConfigurationOptions,
            jsonSerializerOptions);
    }

    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddDistributedCacheCore(services, name);

        services.UseStackExchangeRedisCache(name, redisConfigurationOptions, jsonSerializerOptions);

        return new CachingBuilder(services, name);
    }
}
