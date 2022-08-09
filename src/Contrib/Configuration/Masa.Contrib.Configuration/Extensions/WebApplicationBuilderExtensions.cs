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
        IConfiguration configuration = builder.Configuration;
        bool initialized = false;
        builder.Services.Configure<MasaAppConfigureOptions>(options =>
        {
            if (!initialized)
            {
                var masaConfiguration = builder.Services.BuildServiceProvider().GetService<IMasaConfiguration>();
                if (masaConfiguration != null) configuration = masaConfiguration.Local;
                initialized = true;
            }

            if (string.IsNullOrWhiteSpace(options.AppId))
                options.AppId = configuration.GetConfigurationValue(optionsRelation.Data[nameof(options.AppId)].Variable,
                    () => optionsRelation.Data[nameof(options.AppId)].DefaultValue);

            if (string.IsNullOrWhiteSpace(options.Environment))
                options.Environment = configuration.GetConfigurationValue(optionsRelation.Data[nameof(options.Environment)].Variable,
                    () => optionsRelation.Data[nameof(options.Environment)].DefaultValue);

            if (string.IsNullOrWhiteSpace(options.Cluster))
                options.Cluster = configuration.GetConfigurationValue(optionsRelation.Data[nameof(options.Cluster)].Variable,
                    () => optionsRelation.Data[nameof(options.Cluster)].DefaultValue);
        });
        return builder;
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

    private class InitializeAppConfigurationProvider
    {

    }
}
