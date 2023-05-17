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
        var enableMultiEnvironment = serviceProvider.EnableMultiEnvironment();
        var environment = !enableMultiEnvironment ?
            GetEnvironmentByDisableMultiEnvironment() :
            serviceProvider.GetRequiredService<IMultiEnvironmentContext>().CurrentEnvironment;

        var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        if (!environment.IsNullOrWhiteSpace())
        {
            configurationBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
        }
        if (!enableMultiEnvironment) configurationBuilder.AddEnvironmentVariables();

        configurationBuilderAction?.Invoke(configurationBuilder, serviceProvider);
        return new LocalMasaConfigurationRepository(configurationBuilder.Build(), serviceProvider.GetService<ILoggerFactory>());
    }

    private static string? GetEnvironmentByDisableMultiEnvironment()
    {
        var environment = Environment.GetEnvironmentVariable(ConfigurationConstant.ENVIRONMENT_VARIABLE_NAME);
        return environment;
    }
}
