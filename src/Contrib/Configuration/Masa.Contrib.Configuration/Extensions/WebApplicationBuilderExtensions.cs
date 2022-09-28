// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

#pragma warning disable CS0618
public static class WebApplicationBuilderExtensions
{
    [Obsolete("Use Services.InitializeAppConfiguration() instead")]
    public static WebApplicationBuilder InitializeAppConfiguration(this WebApplicationBuilder builder)
        => builder.InitializeAppConfiguration(null);

    [Obsolete("Use Services.InitializeAppConfiguration() instead")]
    public static WebApplicationBuilder InitializeAppConfiguration(
        this WebApplicationBuilder builder,
        Action<MasaAppConfigureOptionsRelation>? action)
    {
        builder.Services.InitializeAppConfiguration(action);
        return builder;
    }

    [Obsolete("Use Services.AddMasaConfiguration() instead")]
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate = null)
    {
        Action<ConfigurationOptions>? action = null;
        return builder.AddMasaConfiguration(configureDelegate, action);
    }

    [Obsolete("Use Services.AddMasaConfiguration() instead")]
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        params Assembly[] assemblies)
    {
        builder.Services.AddMasaConfiguration(assemblies);
        return builder;
    }

    [Obsolete("Use Services.AddMasaConfiguration() instead")]
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<ConfigurationOptions>? action)
    {
        builder.Services.AddMasaConfiguration(action);
        return builder;
    }

    [Obsolete("Use Services.AddMasaConfiguration() instead")]
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        params Assembly[] assemblies)
    {
        builder.Services.AddMasaConfiguration(
            configureDelegate, assemblies);
        return builder;
    }

    [Obsolete("Use Services.AddMasaConfiguration() instead")]
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        Action<ConfigurationOptions>? action)
    {
        builder.Services.AddMasaConfiguration(configureDelegate, action);
        return builder;
    }

    [Obsolete("Use Services.GetMasaConfiguration() instead")]
    public static IMasaConfiguration GetMasaConfiguration(this WebApplicationBuilder builder)
        => builder.Services.BuildServiceProvider().GetRequiredService<IMasaConfiguration>();
}
#pragma warning restore CS0618
