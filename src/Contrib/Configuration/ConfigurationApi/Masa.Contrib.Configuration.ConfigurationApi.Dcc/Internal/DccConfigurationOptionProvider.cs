// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal class DccConfigurationOptionProvider
{
    private readonly DccConfigurationOptionsCache _dccConfigurationOptionsCache;

    public DccConfigurationOptionProvider(DccConfigurationOptionsCache dccConfigurationOptionsCache)
        => _dccConfigurationOptionsCache = dccConfigurationOptionsCache;

    public DccConfigurationOptions GetOptions(
        IServiceProvider serviceProvider,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        var masaConfigurationEnvironmentProvider = serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
        masaConfigurationEnvironmentProvider.TryGetDefaultEnvironment(serviceProvider, out var environment);
        return GetOptions(serviceProvider, environment ?? string.Empty, dccOptionsFunc);
    }

    private DccConfigurationOptions GetOptions(
        IServiceProvider serviceProvider,
        string environment,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        return _dccConfigurationOptionsCache.GetOrAdd(environment, _ =>
        {
            var dccOptions = dccOptionsFunc == null ?
                serviceProvider.GetRequiredService<IOptions<DccOptions>>().Value :
                dccOptionsFunc.Invoke(serviceProvider);

            var dccConfigurationOptions = dccOptions.ConvertToDccConfigurationOptions();

            dccConfigurationOptions.CheckAndComplementDccConfigurationOptions(serviceProvider);
            return dccConfigurationOptions;
        });
    }
}
