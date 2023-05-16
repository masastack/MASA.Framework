// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class MasaConfigurationEnvironmentProvider
{
    private readonly MasaConfigurationEnvironmentCache _masaConfigurationEnvironmentCache;

    public MasaConfigurationEnvironmentProvider(MasaConfigurationEnvironmentCache masaConfigurationEnvironmentCache)
        => _masaConfigurationEnvironmentCache = masaConfigurationEnvironmentCache;

    public bool TryGetDefaultEnvironment(IServiceProvider serviceProvider, [NotNullWhen(true)] out string? environment)
    {
        environment = _masaConfigurationEnvironmentCache.GetOrAdd(serviceProvider, sp =>
        {
            var isolationOptions = sp.GetRequiredService<IOptions<IsolationOptions>>();
            if (!isolationOptions.Value.EnableMultiEnvironment)
            {
                var globalMasaAppOptions = sp.GetRequiredService<IOptions<MasaAppConfigureOptions>>();
                return globalMasaAppOptions.Value.Environment;
            }

            var multiEnvironmentContext = sp.GetService<IMultiEnvironmentContext>();
            MasaArgumentException.ThrowIfNull(multiEnvironmentContext);
            return multiEnvironmentContext.CurrentEnvironment;
        });
        return !environment.IsNullOrWhiteSpace();
    }
}
