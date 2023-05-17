// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        params Assembly[] assemblies)
        => services.AddMasaConfiguration(assemblies.AsEnumerable());

    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        IEnumerable<Assembly>? assemblies)
        => services.AddMasaConfiguration(optionsBuilder =>
        {
            if (assemblies != null && assemblies.Any())
            {
                optionsBuilder.Assemblies = assemblies;
            }
        });

    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        Action<MasaConfigurationOptionsBuilder>? optionsBuilderConfigure)
    {
        if (services.Any(service => service.ImplementationType == typeof(MasaConfigurationProvider)))
            return services;

        services.AddSingleton<MasaConfigurationProvider>();

        var masaConfigurationOptionsBuilder = new MasaConfigurationOptionsBuilder(services);
        optionsBuilderConfigure?.Invoke(masaConfigurationOptionsBuilder);

        services.TryAddSingleton<IMasaConfigurationChangeListener, DefaultMasaConfigurationChangeListener>();
        services.AddSingleton<MasaConfigurationOptionsCache>();
        services.AddTransient<IMasaConfigurationFactory, DefaultMasaConfigurationFactory>();
        services.AddTransient<IMasaConfiguration>(serviceProvider => serviceProvider.GetRequiredService<IMasaConfigurationFactory>().Create(SectionTypes.All));
        services.TryAddConfigurationEnvironmentProvider();
        services.AddSingleton<AutoMapOptionsProvider>();

        masaConfigurationOptionsBuilder.AddOptions();

        services.Configure<MasaConfigurationOptions>(options =>
        {
            options.AddConfigurationRepository(
                SectionTypes.Local,
                serviceProvider
                    => serviceProvider.GetLocalConfigurationRepository(masaConfigurationOptionsBuilder.ConfigurationBuilderAction));
        });
        return services;
    }

    private static void TryAddConfigurationEnvironmentProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<SingletonService<MasaConfigurationEnvironmentProvider>>(serviceProvider =>
        {
            var masaConfigurationEnvironmentProvider = new MasaConfigurationEnvironmentProvider(serviceProvider);
            return new SingletonService<MasaConfigurationEnvironmentProvider>(masaConfigurationEnvironmentProvider);
        });
        services.TryAddScoped<ScopedService<MasaConfigurationEnvironmentProvider>>(serviceProvider =>
        {
            var masaConfigurationEnvironmentProvider = new MasaConfigurationEnvironmentProvider(serviceProvider);
            return new ScopedService<MasaConfigurationEnvironmentProvider>(masaConfigurationEnvironmentProvider);
        });

        services.TryAddTransient<MasaConfigurationEnvironmentProvider>(serviceProvider =>
        {
            var enableMultiEnvironment = serviceProvider.EnableMultiEnvironment();
            return enableMultiEnvironment ?
                serviceProvider.GetRequiredService<ScopedService<MasaConfigurationEnvironmentProvider>>().Service :
                serviceProvider.GetRequiredService<SingletonService<MasaConfigurationEnvironmentProvider>>().Service;
        });
    }

    public static IMasaConfiguration GetMasaConfiguration(this IServiceCollection services)
        => services.BuildServiceProvider().GetRequiredService<IMasaConfiguration>();

    private sealed class MasaConfigurationProvider
    {

    }
}
