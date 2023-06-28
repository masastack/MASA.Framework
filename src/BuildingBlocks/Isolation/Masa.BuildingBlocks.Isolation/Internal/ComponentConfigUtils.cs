// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Caching.Distributed.StackExchangeRedis")]
[assembly: InternalsVisibleTo("Masa.Contrib.Caching.MultilevelCache")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Storage.ObjectStorage.Aliyun")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

internal static class ComponentConfigUtils
{
    public static List<IsolationConfigurationOptions<TComponentConfig>> GetComponentConfigs<TComponentConfig>(
        IServiceProvider serviceProvider,
        string name,
        string sectionName)
        where TComponentConfig : class
    {
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IsolationOptions<TComponentConfig>>>().Get(name);
        if (optionsMonitor.Data.Count > 0)
            return optionsMonitor.Data;

        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        var rootSectionName = isolationOptions.Value.SectionName;

        var configuration = serviceProvider.GetService<IMasaConfiguration>()?.Local ?? serviceProvider.GetService<IConfiguration>();
        MasaArgumentException.ThrowIfNull(configuration);
        return configuration
            .GetSection(rootSectionName)
            .GetSection(sectionName)
            .Get<List<IsolationConfigurationOptions<TComponentConfig>>>() ?? new();
    }

    /// <summary>
    /// Get runtime configuration information
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="name"></param>
    /// <param name="sectionName"></param>
    /// <param name="defaultFunc"></param>
    /// <typeparam name="TComponentConfig"></typeparam>
    /// <returns></returns>
    public static TComponentConfig GetComponentConfigByExecute<TComponentConfig>(IServiceProvider serviceProvider,
        string name,
        string sectionName,
        Func<TComponentConfig> defaultFunc) where TComponentConfig : class
    {
        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        if (isolationOptions.Value.Enable)
        {
            return serviceProvider
                .GetRequiredService<IIsolationConfigProvider>()
                .GetComponentConfig<TComponentConfig>(sectionName, name) ?? defaultFunc.Invoke();
        }
        return defaultFunc.Invoke();
    }
}
