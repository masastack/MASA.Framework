// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaConfigureNamedOptions<TOptions> : IConfigureNamedOptions<TOptions> where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<(Type, string), ConfigurationRelationOptions> _configurationRelationOptions;

    public MasaConfigureNamedOptions(
        IServiceProvider serviceProvider,
        Dictionary<(Type, string), ConfigurationRelationOptions> configurationRelationOptions)
    {
        _serviceProvider = serviceProvider;
        _configurationRelationOptions = configurationRelationOptions;
    }

    public void Configure(TOptions options) => Configure(Options.DefaultName, options);

    public void Configure(string name, TOptions options)
    {
        MasaArgumentException.ThrowIfNull(name);

        if (!_configurationRelationOptions.TryGetValue((options.GetType(), name), out var relationOption))
            return;

        var masaConfiguration = relationOption.IsRequiredConfigComponent ?
            _serviceProvider.GetRequiredService<IMasaConfigurationFactory>().Create(relationOption.SectionType) :
            _serviceProvider.GetRequiredService<IMasaConfiguration>();

        var configuration = masaConfiguration.GetConfiguration(relationOption.SectionType);
        if (!relationOption.ParentSection.IsNullOrWhiteSpace())
            configuration = configuration.GetSection(relationOption.ParentSection);
        if (!relationOption.Section.IsNullOrWhiteSpace())
            configuration = configuration.GetSection(relationOption.Section);

        configuration.Bind(options);
    }
}
