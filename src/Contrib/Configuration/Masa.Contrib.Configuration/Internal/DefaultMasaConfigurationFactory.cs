// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class DefaultMasaConfigurationFactory : IMasaConfigurationFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MasaConfigurationOptionsCache _masaConfigurationOptionsCache;
    private readonly Lazy<IConfigurationApi> _configurationApiLazy;
    private readonly IOptions<MasaConfigurationOptions> _options;

    public DefaultMasaConfigurationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _masaConfigurationOptionsCache = serviceProvider.GetRequiredService<MasaConfigurationOptionsCache>();
        _configurationApiLazy = new Lazy<IConfigurationApi>(serviceProvider.GetRequiredService<IConfigurationApi>);

        _options = serviceProvider.GetRequiredService<IOptions<MasaConfigurationOptions>>();
    }

    public IMasaConfiguration Create(SectionTypes sectionType)
    {
        var configurationEnvironmentProvider = _serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
        configurationEnvironmentProvider.TryGetDefaultEnvironment(_serviceProvider, out var environment);
        return _masaConfigurationOptionsCache.GetOrAdd((environment ?? string.Empty, sectionType), item =>
        {
            var configuration = GetConfiguration(item.SectionType);
            return new DefaultMasaConfiguration(configuration, _configurationApiLazy);
        });
    }

    private IConfiguration GetConfiguration(SectionTypes sectionType)
    {
        List<IConfigurationRepository> repositories = new List<IConfigurationRepository>();

        foreach (var handler in _options.Value.ConfigurationRepositoryHandlers)
        {
            if ((sectionType & handler.SectionType) == handler.SectionType)
            {
                var configurationRepository = handler.Func.Invoke(_serviceProvider);
                repositories.Add(configurationRepository);
            }
        }

        var source = new MasaConfigurationSource(repositories);
        var configuration = new ConfigurationBuilder()
            .Add(source)
            .Build();

        return configuration;
    }
}
