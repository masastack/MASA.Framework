namespace MASA.Contrib.Service.MinimalAPIs;
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddServices<ServiceBase>();

        return services;
    }
}
