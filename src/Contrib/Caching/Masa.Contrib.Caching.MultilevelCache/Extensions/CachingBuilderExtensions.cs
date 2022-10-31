// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CachingBuilderExtensions
{
    /// <summary>
    /// Add multi-level cache
    /// </summary>
    /// <param name="cachingBuilder"></param>
    /// <param name="sectionName">MultilevelCache node name, not required, default: MultilevelCache(Use local configuration)</param>
    /// <param name="isReset">Whether to reset the MemoryCache after configuration changes</param>
    /// <returns></returns>
    [Obsolete(
        "cachingBuilder.AddMultilevelCache has expired, please use services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddMultilevelCache(
        this ICachingBuilder cachingBuilder,
        string sectionName = Const.DEFAULT_SECTION_NAME,
        bool isReset = false)
    {
        cachingBuilder.Services.AddMultilevelCache(cachingBuilder.Name, sectionName, isReset);
        return cachingBuilder;
    }

    [Obsolete(
        "cachingBuilder.AddMultilevelCache has expired, please use services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddMultilevelCache(this ICachingBuilder cachingBuilder, Action<MultilevelCacheOptions> action)
    {
        var multilevelCacheOptions = new MultilevelCacheOptions();
        action.Invoke(multilevelCacheOptions);
        return cachingBuilder.AddMultilevelCache(multilevelCacheOptions);
    }

    [Obsolete(
        "cachingBuilder.AddMultilevelCache has expired, please use services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddMultilevelCache(this ICachingBuilder cachingBuilder, MultilevelCacheOptions multilevelCacheOptions)
    {
        ArgumentNullException.ThrowIfNull(cachingBuilder);

        cachingBuilder.Services.AddMultilevelCache(cachingBuilder.Name, multilevelCacheOptions);

        return cachingBuilder;
    }
}
