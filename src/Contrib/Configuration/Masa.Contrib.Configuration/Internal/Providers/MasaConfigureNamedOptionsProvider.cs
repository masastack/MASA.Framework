// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaConfigureNamedOptionsProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Lazy<IAutoMapOptionsByConfigurationApiProvider?> _autoMapOptionsByConfigurationApiProviderLazy;

    private IAutoMapOptionsByConfigurationApiProvider? AutoMapOptionsByConfigurationApiProvider
        => _autoMapOptionsByConfigurationApiProviderLazy.Value;

    private readonly Lazy<AutoMapOptionsProvider> _autoMapOptionsProviderLazy;
    private AutoMapOptionsProvider AutoMapOptionsProvider => _autoMapOptionsProviderLazy.Value;
    private HashSet<Type> AutoMapTypes => AutoMapOptionsProvider.AutoMapTypes;

    private Dictionary<(Type ObjectType, string OptionsName), ConfigurationRelationOptions> CompleteAutoMapOptions
        => AutoMapOptionsProvider.CompleteAutoMapOptions;

    public MasaConfigureNamedOptionsProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _autoMapOptionsByConfigurationApiProviderLazy = new(serviceProvider.GetService<IAutoMapOptionsByConfigurationApiProvider>);
        _autoMapOptionsProviderLazy = new Lazy<AutoMapOptionsProvider>(serviceProvider.GetRequiredService<AutoMapOptionsProvider>);
    }

    public void Configure<[DynamicallyAccessedMembers(ConfigurationConstant.DYNAMICALLY_ACCESSED_MEMBERS)] TOptions>(
        string name,
        TOptions options)
        where TOptions : class
    {
        MasaArgumentException.ThrowIfNull(name);

        if(ConfigurationUtils.IsSkipAutoOptions(typeof(TOptions)))
            return;

        var optionsType = typeof(TOptions);
        if (!AutoMapTypes.Contains(optionsType))
            return;

        if (CompleteAutoMapOptions.TryGetValue((options.GetType(), name), out var relationOptions))
        {
            Configure(relationOptions, options);
            return;
        }

        if (AutoMapOptionsByConfigurationApiProvider != null &&
            AutoMapOptionsByConfigurationApiProvider.TryGetAutoMapOptions(options.GetType(), name, out relationOptions))
        {
            Configure(relationOptions, options);
        }
    }

    private void Configure<[DynamicallyAccessedMembers(ConfigurationConstant.DYNAMICALLY_ACCESSED_MEMBERS)] TOptions>(
        ConfigurationRelationOptions relationOption, TOptions options)
        where TOptions : class
    {
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
