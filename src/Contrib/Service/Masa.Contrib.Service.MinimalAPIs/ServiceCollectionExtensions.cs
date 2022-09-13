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
    public static WebApplication AddServices(this WebApplicationBuilder builder, Assembly[]? assemblies = null)
        => builder.Services.AddServices(builder, assemblies);

    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <param name="assemblies">The assembly collection where the MinimalApi service is located</param>
    /// <returns></returns>
    public static WebApplication AddServices(this IServiceCollection services, WebApplicationBuilder builder, Assembly[]? assemblies = null)
    {
        if (services.All(service => service.ImplementationType != typeof(MinimalApisMarkerService)))
        {
            services.AddSingleton<MinimalApisMarkerService>();
            services.TryAddScoped(sp => services);

            services.AddSingleton(new Lazy<WebApplication>(builder.Build, LazyThreadSafetyMode.ExecutionAndPublication))
                .AddTransient(serviceProvider => serviceProvider.GetRequiredService<Lazy<WebApplication>>().Value);

            services.AddServices<ServiceBase>(true, (_, serviceInstance) =>
            {
                var instance = (ServiceBase)serviceInstance;
                if (!instance.DisableRestful) instance.AutoMapRouter();
            }, assemblies ?? AppDomain.CurrentDomain.GetAssemblies());
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
