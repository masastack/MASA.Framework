// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <param name="assemblies">The assembly collection where the MinimalApi service is located</param>
    /// <returns></returns>
    public static WebApplication AddServices(
        this WebApplicationBuilder builder,
        Assembly[]? assemblies = null)
        => builder.Services.AddServices(builder, assemblies);

    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static WebApplication AddServices(
        this WebApplicationBuilder builder,
        Action<ServiceMapOptions> action)
        => builder.Services.AddServices(builder, action);

    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <param name="assemblies">The assembly collection where the MinimalApi service is located</param>
    /// <returns></returns>
    public static WebApplication AddServices(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        Assembly[]? assemblies = null)
        => services.AddServices(builder, options =>
        {
            if (assemblies != null) options.Assemblies = assemblies;
        });

    public static WebApplication AddServices(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        Action<ServiceMapOptions> action)
    {
        if (services.All(service => service.ImplementationType != typeof(MinimalApisMarkerService)))
        {
            services.AddSingleton<MinimalApisMarkerService>();

            services.Configure(action);

#pragma warning disable CA1822
            //todo: Version 1.0 will be removed
            services.TryAddScoped(sp => services);
#pragma warning restore CA1822

            services.AddSingleton(new Lazy<WebApplication>(builder.Build, LazyThreadSafetyMode.ExecutionAndPublication))
                .AddTransient(serviceProvider => serviceProvider.GetRequiredService<Lazy<WebApplication>>().Value);

            MasaApp.Services = services;

            MasaApp.Build(services.BuildServiceProvider());
            var serviceMapOptions = MasaApp.GetRequiredService<IOptions<ServiceMapOptions>>().Value;
            services.AddServices<ServiceBase>(true, (_, serviceInstance) =>
            {
                var instance = (ServiceBase)serviceInstance;
                if (!instance.DisableRestful) instance.AutoMapRoute(serviceMapOptions, serviceMapOptions.Pluralization);
            }, serviceMapOptions.Assemblies);
        }

        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<WebApplication>();
        MasaApp.Build(app.Services);
        return app;
    }

    private sealed class MinimalApisMarkerService
    {

    }
}
