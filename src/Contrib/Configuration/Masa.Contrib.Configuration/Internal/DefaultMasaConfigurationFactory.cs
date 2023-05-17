// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class DefaultMasaConfigurationFactory : IMasaConfigurationFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MasaConfigurationOptionsCache _masaConfigurationOptionsCache;
    private readonly Lazy<IConfigurationApi?> _configurationApiLazy;
    private readonly IMasaConfigurationChangeListener _masaConfigurationChangeListener;

    public DefaultMasaConfigurationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _masaConfigurationOptionsCache = serviceProvider.GetRequiredService<MasaConfigurationOptionsCache>();
        _configurationApiLazy = new Lazy<IConfigurationApi?>(serviceProvider.GetService<IConfigurationApi>);

        _masaConfigurationChangeListener = _serviceProvider.GetRequiredService<IMasaConfigurationChangeListener>();
    }

    public IMasaConfiguration Create(SectionTypes sectionType)
    {
        var configurationEnvironmentProvider = _serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
        var currentEnvironment = configurationEnvironmentProvider.GetCurrentEnvironment(_serviceProvider.EnableMultiEnvironment());
        return _masaConfigurationOptionsCache.GetOrAdd((currentEnvironment, sectionType), item =>
        {
            var configuration = GetConfiguration(item.Environment, item.SectionType);
            return new DefaultMasaConfiguration(configuration, _configurationApiLazy);
        });
    }

    private IConfiguration GetConfiguration(string environment, SectionTypes sectionType)
    {
        var options = _serviceProvider.GetRequiredService<IOptions<MasaConfigurationOptions>>();

        var repositories = (
            from handler in options.Value.ConfigurationRepositoryHandlers
            where (sectionType & handler.SectionType) == handler.SectionType
            select handler.Func.Invoke(_serviceProvider)).ToList();

        var source = new MasaConfigurationSource(repositories);
        var configuration = new ConfigurationBuilder()
            .Add(source)
            .Build();

        ChangeToken.OnChange(() => configuration.GetReloadToken(), () =>
        {
            _masaConfigurationChangeListener.OnChanged(new MasaConfigurationChangeOptions(configuration, environment));
        });

        return configuration;
    }
}
