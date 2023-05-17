// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

/// <summary>
/// The provider of IOptions provided by MASA Framework,
/// Select the corresponding provider according to the open state of the multi-environment
/// </summary>
/// <typeparam name="TOptions"></typeparam>
internal class MasaUnnamedOptionsProvider<[DynamicallyAccessedMembers(ConfigurationConstant.DYNAMICALLY_ACCESSED_MEMBERS)] TOptions>
    where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MasaUnnamedOptionsCache<TOptions> _optionsCache;

    public MasaUnnamedOptionsProvider(IServiceProvider serviceProvider, MasaUnnamedOptionsCache<TOptions> optionsCache)
    {
        _serviceProvider = serviceProvider;
        _optionsCache = optionsCache;
    }

    public TOptions GetOptions()
    {
        var environmentProvider = _serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
        var currentEnvironment = environmentProvider.GetCurrentEnvironment(true);
        return _optionsCache.GetOrAdd(currentEnvironment, env =>
        {
            var factory = _serviceProvider.GetRequiredService<IOptionsFactory<TOptions>>();
            return factory.Create(Options.DefaultName);
        });
    }
}
