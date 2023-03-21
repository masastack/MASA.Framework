// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Caching.Distributed.StackExchangeRedis")]
[assembly: InternalsVisibleTo("Masa.Contrib.Caching.MultilevelCache")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Storage.ObjectStorage.Aliyun")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

internal static class ModuleConfigUtils
{
    public static List<IsolationConfigurationOptions<TModuleConfig>> GetModuleConfigs<TModuleConfig>(
        IServiceProvider serviceProvider,
        string name,
        string sectionName)
        where TModuleConfig : class
    {
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IsolationOptions<TModuleConfig>>>().Get(name);
        if (optionsMonitor.Data.Count > 0)
            return optionsMonitor.Data;

        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        var rootSectionName = isolationOptions.Value.SectionName;

        var configuration = serviceProvider.GetService<IMasaConfiguration>()?.Local ?? serviceProvider.GetService<IConfiguration>();
        MasaArgumentException.ThrowIfNull(configuration);
        return configuration
            .GetSection(rootSectionName)
            .GetSection(sectionName)
            .Get<List<IsolationConfigurationOptions<TModuleConfig>>>() ?? new();
    }

    /// <summary>
    /// Get runtime configuration information
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="name"></param>
    /// <param name="sectionName"></param>
    /// <param name="defaultFunc"></param>
    /// <typeparam name="TModuleConfig"></typeparam>
    /// <returns></returns>
    public static TModuleConfig GetModuleConfigByExecute<TModuleConfig>(IServiceProvider serviceProvider,
        string name,
        string sectionName,
        Func<TModuleConfig> defaultFunc) where TModuleConfig : class
    {
        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        if (isolationOptions.Value.Enable)
        {
            return serviceProvider
                .GetRequiredService<IIsolationConfigProvider>()
                .GetModuleConfig<TModuleConfig>(sectionName, name) ?? defaultFunc.Invoke();
        }
        return defaultFunc.Invoke();
    }
}
