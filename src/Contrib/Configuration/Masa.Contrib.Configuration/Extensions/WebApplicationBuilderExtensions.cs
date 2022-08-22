// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder InitializeAppConfiguration(this WebApplicationBuilder builder)
        => builder.InitializeAppConfiguration(null);

    public static WebApplicationBuilder InitializeAppConfiguration(
        this WebApplicationBuilder builder,
        Action<MasaAppConfigureOptionsRelation>? action)
    {
        if (builder.Services.Any(service => service.ImplementationType == typeof(InitializeAppConfigurationProvider)))
            return builder;

        builder.Services.AddSingleton<InitializeAppConfigurationProvider>();

        MasaApp.Services = builder.Services;

        MasaAppConfigureOptionsRelation optionsRelation = new();
        action?.Invoke(optionsRelation);
        IConfiguration? migrateConfiguration = null;
        bool initialized = false;
        builder.Services.Configure<MasaAppConfigureOptions>(options =>
        {
            if (!initialized)
            {
                var masaConfiguration = builder.Services.BuildServiceProvider().GetService<IMasaConfiguration>();
                if (masaConfiguration != null) migrateConfiguration = masaConfiguration.Local;
                initialized = true;
            }

            if (string.IsNullOrWhiteSpace(options.AppId))
                options.AppId = GetConfigurationValue(
                    optionsRelation.GetValue(nameof(options.AppId)),
                    builder.Configuration,
                    migrateConfiguration);

            if (string.IsNullOrWhiteSpace(options.Environment))
                options.Environment = GetConfigurationValue(
                    optionsRelation.GetValue(nameof(options.Environment)),
                    builder.Configuration,
                    migrateConfiguration);

            if (string.IsNullOrWhiteSpace(options.Cluster))
                options.Cluster = GetConfigurationValue(
                    optionsRelation.GetValue(nameof(options.Cluster)),
                    builder.Configuration,
                    migrateConfiguration);

            foreach (var key in optionsRelation.GetKeys())
            {
                options.TryAdd(key, GetConfigurationValue(
                    optionsRelation.GetValue(key),
                    builder.Configuration,
                    migrateConfiguration));
            }
        });
        return builder;
    }

    private static string GetConfigurationValue((string Variable, string DefaultValue) defaultConfig,
        IConfiguration configuration,
        IConfiguration? migrateConfiguration)
    {
        var value = configuration[defaultConfig.Variable];
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        if (migrateConfiguration != null)
            value = migrateConfiguration[defaultConfig.Variable];
        if (string.IsNullOrWhiteSpace(value))
            value = defaultConfig.DefaultValue;
        return value;
    }

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate = null)
    {
        Action<ConfigurationOptions>? action = null;
        return builder.AddMasaConfiguration(configureDelegate, action);
    }

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        params Assembly[] assemblies)
        => builder.AddMasaConfiguration(
            null,
            options => options.Assemblies = assemblies);

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<ConfigurationOptions>? action)
        => builder.AddMasaConfiguration(null, action);

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        params Assembly[] assemblies)
        => builder.AddMasaConfiguration(
            configureDelegate,
            options => options.Assemblies = assemblies);

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        Action<ConfigurationOptions>? action)
    {
        builder.InitializeAppConfiguration();

        IConfigurationRoot masaConfiguration =
            builder.Services.CreateMasaConfiguration(
                configureDelegate,
                builder.Configuration,
                action);

        if (!masaConfiguration.Providers.Any())
            return builder;

        Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureAppConfiguration(
            builder.Host,
            configBuilder => configBuilder.Sources.Clear());
        builder.Configuration.AddConfiguration(masaConfiguration);

        return builder;
    }

    public static IMasaConfiguration GetMasaConfiguration(this WebApplicationBuilder builder)
        => builder.Services.BuildServiceProvider().GetRequiredService<IMasaConfiguration>();

    private sealed class InitializeAppConfigurationProvider
    {

    }
}
