using System.Linq;

namespace MASA.Contrib.Service.MinimalAPIs;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <returns></returns>
    public static WebApplication AddServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (services.All(service => service.ImplementationType != typeof(MinimalApisMarkerService)))
        {
            services.AddSingleton<MinimalApisMarkerService>();
            services.TryAddScoped(sp => services);

            services.AddSingleton(new Lazy<WebApplication>(() => builder.Build(), LazyThreadSafetyMode.ExecutionAndPublication))
                .AddTransient(serviceProvider => serviceProvider.GetRequiredService<Lazy<WebApplication>>().Value);

            services.AddServices<ServiceBase>(true);
        }

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<WebApplication>();
    }

    private class MinimalApisMarkerService
    {

    }
}
