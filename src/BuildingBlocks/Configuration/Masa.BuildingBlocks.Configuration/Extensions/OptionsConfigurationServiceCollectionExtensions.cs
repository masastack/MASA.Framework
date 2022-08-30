// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsConfigurationServiceCollectionExtensions
{
    /// <summary>
    /// Only consider using MasaConfiguration and database configuration using local configuration
    /// When using MasaConfiguration and the database configuration is stored in ConfigurationApi, you need to specify the mapping relationship in Configuration by yourself
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        IConfiguration? configuration = serviceProvider.GetService<Masa.BuildingBlocks.Configuration.IMasaConfiguration>()?.Local ??
            serviceProvider.GetService<IConfiguration>();
        if (configuration == null)
            return services;

        string name = Microsoft.Extensions.Options.Options.DefaultName;
        var configurationSection = configuration.GetSection(sectionName);
        if (!configurationSection.Exists())
            return services;

        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, configuration));
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name,
            configuration, _ =>
            {
            }));
        return services;
    }
}
