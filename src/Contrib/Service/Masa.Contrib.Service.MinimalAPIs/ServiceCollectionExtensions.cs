// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
        Action<ServiceGlobalRouteOptions> action)
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
        Action<ServiceGlobalRouteOptions> action)
    {
        if (services.All(service => service.ImplementationType != typeof(MinimalApisMarkerService)))
        {
            services.AddSingleton<MinimalApisMarkerService>();

            services.AddHttpContextAccessor();
            services.Configure(action);

            services.TryAddScoped(sp => services); // Version 1.0 will be removed

            services.AddSingleton(new Lazy<WebApplication>(builder.Build, LazyThreadSafetyMode.ExecutionAndPublication))
                .AddTransient(serviceProvider => serviceProvider.GetRequiredService<Lazy<WebApplication>>().Value);

            MasaApp.TrySetServiceCollection(services);

            MasaApp.Build(services.BuildServiceProvider());
            var serviceMapOptions = MasaApp.GetRequiredService<IOptions<ServiceGlobalRouteOptions>>().Value;
            services.AddServices<ServiceBase>(true, (_, serviceInstance) =>
            {
                var instance = (ServiceBase)serviceInstance;
                if (instance.RouteOptions.DisableAutoMapRoute ?? serviceMapOptions.DisableAutoMapRoute ?? false)
                    return;

                instance.AutoMapRoute(serviceMapOptions, serviceMapOptions.Pluralization);
            }, serviceMapOptions.Assemblies);
        }

        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<WebApplication>();
        if (MasaApp.GetJsonSerializerOptions() == null)
            MasaApp.TrySetJsonSerializerOptions(app.Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);

        MasaApp.Build(app.Services);
        return app;
    }

    private sealed class MinimalApisMarkerService
    {

    }
}
