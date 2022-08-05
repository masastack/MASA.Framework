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

        MasaAppConfigureOptionsRelation optionsRelation = new();
        action?.Invoke(optionsRelation);
        IConfiguration configuration = builder.Configuration;
        bool isInitialize = false;
        builder.Services.Configure<MasaAppConfigureOptions>(options =>
        {
            if (!isInitialize)
            {
                var masaConfiguration = builder.Services.BuildServiceProvider().GetService<IMasaConfiguration>();
                if (masaConfiguration != null) configuration = masaConfiguration.Local;
                isInitialize = true;
            }

            if (string.IsNullOrWhiteSpace(options.AppId))
                options.AppId = configuration.GetConfigurationValue(optionsRelation.DataVariables[nameof(options.AppId)],
                    () => optionsRelation.DataDefaultValue[nameof(options.AppId)]);

            if (string.IsNullOrWhiteSpace(options.Environment))
                options.Environment = configuration.GetConfigurationValue(optionsRelation.DataVariables[nameof(options.Environment)],
                    () => optionsRelation.DataDefaultValue[nameof(options.Environment)]);

            if (string.IsNullOrWhiteSpace(options.Cluster))
                options.Cluster = configuration.GetConfigurationValue(optionsRelation.DataVariables[nameof(options.Cluster)],
                    () => optionsRelation.DataDefaultValue[nameof(options.Cluster)]);
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
