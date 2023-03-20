// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

internal static class ModuleConfigUtils
{
    public static List<IsolationConfigurationOptions<TModuleConfig>> GetModules<TModuleConfig>(
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
}
