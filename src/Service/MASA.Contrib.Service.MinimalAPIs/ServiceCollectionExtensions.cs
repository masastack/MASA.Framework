using System.Threading;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <returns></returns>
    public static IServiceCollection AddLazyWebApplication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (services.Any(s => s.ImplementationType == typeof(MinimalApisMarkerService))) return services;

        services.AddSingleton<MinimalApisMarkerService>();

        services.AddSingleton(new Lazy<WebApplication>(() => builder.Build(), LazyThreadSafetyMode.ExecutionAndPublication))
            .AddTransient(serviceProvider => serviceProvider.GetRequiredService<Lazy<WebApplication>>().Value);

        return services;
    }

    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <returns></returns>
    public static void AddServices(this IServiceCollection services)
    {
        if (!services.Any(s => s.ImplementationType == typeof(MinimalApisMarkerService)))
        {
            throw new Exception("Please call AddLazyWebApplication first.");
        }

        if (services.BuildServiceProvider().GetService<IServiceCollection>() is null)
        {
            services.AddScoped(serviceProvider => services);
        }

        services.AddServices<ServiceBase>(true);
    }

    private class MinimalApisMarkerService
    {

    }
}
