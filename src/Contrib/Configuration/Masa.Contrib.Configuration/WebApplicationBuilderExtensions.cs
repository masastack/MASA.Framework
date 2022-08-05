// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder InitializeAppConfiguration(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        builder.Services.Configure<MasaAppConfigureOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.AppId))
                options.AppId = configuration.GetConfigurationValue(nameof(MasaAppConfigureOptions.AppId),
                    () => (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(".", "-"));

            if (string.IsNullOrWhiteSpace(options.Environment))
                options.Environment = configuration.GetConfigurationValue("ASPNETCORE_ENVIRONMENT",
                    () => "Production");

            if (string.IsNullOrWhiteSpace(options.Cluster))
                options.Environment = configuration.GetConfigurationValue(nameof(MasaAppConfigureOptions.Cluster),
                    () => "Default");
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
}
