using MASA.BuildingBlocks.Service.MinimalAPIs;

namespace MASA.Contrib.Service.MinimalAPIs;
public class ServiceBase : IService
{
    public IServiceCollection Services { get; protected set; }

    public ServiceBase(IServiceCollection services) => Services = services;

    public TService? GetService<TService>() => Services.BuildServiceProvider().GetService<TService>();

    public TService GetRequiredService<TService>()
        where TService : notnull
        => Services.BuildServiceProvider().GetRequiredService<TService>();
}
