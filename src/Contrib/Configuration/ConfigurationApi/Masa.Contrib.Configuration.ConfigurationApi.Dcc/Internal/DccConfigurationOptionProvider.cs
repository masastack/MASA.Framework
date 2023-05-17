// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

/// <summary>
/// Get Dcc configuration information
/// </summary>
internal class DccConfigurationOptionProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DccConfigurationOptionsCache _dccConfigurationOptionsCache;
    private readonly string _currentEnvironment;

    public DccConfigurationOptionProvider(IServiceProvider serviceProvider)
    {
        _dccConfigurationOptionsCache = serviceProvider.GetRequiredService<DccConfigurationOptionsCache>();
        _serviceProvider = serviceProvider;

        var masaConfigurationEnvironmentProvider = _serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
        _currentEnvironment = masaConfigurationEnvironmentProvider.GetCurrentEnvironment(serviceProvider.EnableMultiEnvironment());
    }

    public DccConfigurationOptions GetOptions(
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        return GetOptions(_serviceProvider, _currentEnvironment, dccOptionsFunc);
    }

    private DccConfigurationOptions GetOptions(
        IServiceProvider serviceProvider,
        string currentEnvironment,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        var options = _dccConfigurationOptionsCache.GetOrAdd(currentEnvironment, environment =>
        {
            var dccOptions = dccOptionsFunc == null ?
                serviceProvider.GetRequiredService<IOptions<DccOptions>>().Value :
                dccOptionsFunc.Invoke(serviceProvider);

            var dccConfigurationOptions = dccOptions.ConvertToDccConfigurationOptions();
            dccConfigurationOptions.CheckAndComplementDccConfigurationOptions(environment, serviceProvider);
            return dccConfigurationOptions;
        });
        return options;
    }
}
