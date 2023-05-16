// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection InitializeAppConfiguration(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        return services.InitializeGlobalAppConfiguration(configuration);
    }

    public static IServiceCollection InitializeGlobalAppConfiguration(this IServiceCollection services, IConfiguration? configuration)
    {
        if (services.Any(service => service.ImplementationType == typeof(InitializeAppConfigurationProvider)))
            return services;

        services.AddSingleton<InitializeAppConfigurationProvider>();

        MasaApp.TrySetServiceCollection(services);

        services.Configure<MasaAppConfigureOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.AppId))
                options.AppId = GetConfigurationValue(
                    options.GetVariable(nameof(options.AppId)),
                    configuration);

            if (string.IsNullOrWhiteSpace(options.Environment))
                options.Environment = GetConfigurationValue(
                    options.GetVariable(nameof(options.Environment)),
                    configuration);

            if (string.IsNullOrWhiteSpace(options.Cluster))
                options.Cluster = GetConfigurationValue(
                    options.GetVariable(nameof(options.Cluster)),
                    configuration);

            foreach (var key in options.GetVariableKeys())
            {
                options.TryAdd(key, GetConfigurationValue(options.GetVariable(key), configuration));
            }
        });
        return services;
    }

    private static string GetConfigurationValue(VariableInfo? variableInfo, IConfiguration? configuration)
    {
        var value = string.Empty;
        if (variableInfo == null) return value;

        if (configuration != null)
        {
            value = configuration[variableInfo.Variable];
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        if (string.IsNullOrWhiteSpace(value))
            value = variableInfo.DefaultValue;
        return value;
    }

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
        services.InitializeAppConfiguration();

        if (services.Any(service => service.ImplementationType == typeof(MasaConfigurationProvider)))
            return services;

        services.AddSingleton<MasaConfigurationProvider>();

        var masaConfigurationOptionsBuilder = new MasaConfigurationOptionsBuilder(services);
        optionsBuilderConfigure?.Invoke(masaConfigurationOptionsBuilder);

        services.AddSingleton<MasaConfigurationOptionsCache>();
        services.AddTransient<IConfigurationApi, DefaultConfigurationApi>();
        services.AddTransient<IMasaConfigurationFactory, DefaultMasaConfigurationFactory>();
        services.AddTransient<IMasaConfiguration>(serviceProvider
            => serviceProvider.GetRequiredService<IMasaConfigurationFactory>().Create(SectionTypes.All));
        services.TryAddConfigurationEnvironmentProvider();

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
        services.TryAddTransient<MasaConfigurationEnvironmentCache>();

        services.TryAddSingleton<SingletonService<MasaConfigurationEnvironmentProvider>>(serviceProvider =>
        {
            var masaConfigurationEnvironmentCache = serviceProvider.GetRequiredService<MasaConfigurationEnvironmentCache>();
            var masaConfigurationEnvironmentProvider = new MasaConfigurationEnvironmentProvider(masaConfigurationEnvironmentCache);
            return new SingletonService<MasaConfigurationEnvironmentProvider>(masaConfigurationEnvironmentProvider);
        });
        services.TryAddScoped<ScopedService<MasaConfigurationEnvironmentProvider>>(serviceProvider =>
        {
            var masaConfigurationEnvironmentCache = serviceProvider.GetRequiredService<MasaConfigurationEnvironmentCache>();
            var masaConfigurationEnvironmentProvider = new MasaConfigurationEnvironmentProvider(masaConfigurationEnvironmentCache);
            return new ScopedService<MasaConfigurationEnvironmentProvider>(masaConfigurationEnvironmentProvider);
        });

        services.TryAddScoped<MasaConfigurationEnvironmentProvider>(serviceProvider =>
        {
            var enableMultiEnvironment = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>().Value.EnableMultiEnvironment;
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

    private sealed class InitializeAppConfigurationProvider
    {

    }
}
