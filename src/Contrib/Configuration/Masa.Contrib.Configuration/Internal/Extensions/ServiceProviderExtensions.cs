// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

[ExcludeFromCodeCoverage]
internal static class ServiceProviderExtensions
{
    public static IConfigurationRepository GetLocalConfigurationRepository(
        this IServiceProvider serviceProvider,
        Action<IConfigurationBuilder, IServiceProvider>? configurationBuilderAction)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        var masaConfigurationEnvironmentProvider = serviceProvider.GetRequiredService<MasaConfigurationEnvironmentProvider>();
        if (masaConfigurationEnvironmentProvider.TryGetDefaultEnvironment(serviceProvider, out var env))
        {
            configurationBuilder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);
        }
        configurationBuilderAction?.Invoke(configurationBuilder, serviceProvider);
        return new LocalMasaConfigurationRepository(configurationBuilder.Build(), serviceProvider.GetService<ILoggerFactory>());
    }
}
