// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal class DefaultAutoMapOptionsByConfigurationApiProvider : IAutoMapOptionsByConfigurationApiProvider
{
    private readonly MemoryCache<string, Dictionary<(Type, string), ConfigurationRelationOptions>> _memoryCache;

    private readonly Lazy<AutoMapOptionsProvider> _autoMapOptionsProviderLazy;
    public AutoMapOptionsProvider AutoMapOptionsProvider => _autoMapOptionsProviderLazy.Value;

    /// <summary>
    /// Incomplete object mapping collection, need to complete the corresponding AppId later
    /// </summary>
    private Dictionary<(Type ObjectType, string OptionsName), ConfigurationRelationOptions> CompleteAutoMapOptions
        => AutoMapOptionsProvider.IncompleteAutoMapOptions;

    private readonly Lazy<DccConfigurationOptionProvider> _dccConfigurationOptionProviderLazy;
    private DccConfigurationOptionProvider DccConfigurationOptionProvider => _dccConfigurationOptionProviderLazy.Value;

    private readonly string _currentEnvironment;
    private readonly Func<IServiceProvider, DccOptions>? _dccOptionsFunc;

    public DefaultAutoMapOptionsByConfigurationApiProvider(
        IServiceProvider serviceProvider,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        _memoryCache = new();
        _autoMapOptionsProviderLazy = new(serviceProvider.GetRequiredService<AutoMapOptionsProvider>);
        _dccOptionsFunc = dccOptionsFunc;

        var enableMultiEnvironment = serviceProvider.EnableMultiEnvironment();
        var masaConfigurationEnvironmentProvider = serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
        _currentEnvironment = masaConfigurationEnvironmentProvider.GetCurrentEnvironment(enableMultiEnvironment);
        _dccConfigurationOptionProviderLazy = new(serviceProvider.GetRequiredService<DccConfigurationOptionProvider>);
    }

    public bool TryGetAutoMapOptions(Type optionsType, string optionsName,
        [NotNullWhen(true)] out ConfigurationRelationOptions? autoMapOptions)
    {
        var allAutoMapOptions = GetAllAutoMapOptionsByEnvironment();
        return allAutoMapOptions.TryGetValue((optionsType, optionsName), out autoMapOptions);
    }

    private Dictionary<(Type, string), ConfigurationRelationOptions> GetAllAutoMapOptionsByEnvironment()
    {
        return _memoryCache.GetOrAdd(_currentEnvironment, env =>
        {
            var autoMapOptions = new Dictionary<(Type, string), ConfigurationRelationOptions>(CompleteAutoMapOptions);
            foreach (var item in autoMapOptions)
            {
                item.Value.ParentSection = DccConfigurationOptionProvider.GetOptions(_dccOptionsFunc).DefaultSection.AppId;
            }
            return autoMapOptions;
        });
    }
}
