// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Builder;

#pragma warning disable CS0618
public static class WebApplicationBuilderExtensions
{
    [Obsolete("Use Services.InitializeAppConfiguration() instead")]
    public static WebApplicationBuilder InitializeAppConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.InitializeAppConfiguration();
        return builder;
    }

    [Obsolete("Use Services.AddMasaConfiguration() instead")]
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        IEnumerable<Assembly>? assemblies = null)
    {
        builder.Services.AddMasaConfiguration(assemblies);
        return builder;
    }

    [Obsolete("Use Services.AddMasaConfiguration() instead")]
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        Action<ConfigurationOptions>? action = null)
    {
        builder.Services.AddMasaConfiguration(configureDelegate, action);
        return builder;
    }

    [Obsolete("Use Services.GetMasaConfiguration() instead")]
    public static IMasaConfiguration GetMasaConfiguration(this WebApplicationBuilder builder)
        => builder.Services.BuildServiceProvider().GetRequiredService<IMasaConfiguration>();
}
#pragma warning restore CS0618
