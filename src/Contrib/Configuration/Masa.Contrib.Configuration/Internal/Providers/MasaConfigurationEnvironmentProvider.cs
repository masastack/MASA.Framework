// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaConfigurationEnvironmentProvider
{
    private string? _environment;
    private readonly IServiceProvider _serviceProvider;
    private readonly object _lock;

    public MasaConfigurationEnvironmentProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _lock = new();
    }

    public string GetCurrentEnvironment(bool enableMultiEnvironment)
    {
        var environment= _environment;
        if (environment != null)
            return environment;

        lock (_lock)
        {
            if (_environment != null)
                return _environment;

            if (!enableMultiEnvironment)
            {
                environment = ConfigurationUtils.GetEnvironmentWhenDisableMultiEnvironment();
            }
            else
            {
                // Use the environment information of the current user context when multi-environment is enabled
                var multiEnvironmentContext = _serviceProvider.GetService<IMultiEnvironmentContext>();
                MasaArgumentException.ThrowIfNull(multiEnvironmentContext);
                environment = multiEnvironmentContext.CurrentEnvironment;
            }
            _environment = environment;
        }
        return environment;
    }
}
