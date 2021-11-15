namespace MASA.Contrib.Service.MinimalAPIs;

public class ServiceBase : IService
{
    private ServiceProvider _serviceProvider = default!;

    public WebApplication App => _serviceProvider.GetRequiredService<WebApplication>();

    public IServiceCollection Services { get; protected set; }

    public ServiceBase(IServiceCollection services)
    {
        Services = services;
        _serviceProvider = services.BuildServiceProvider();
    }

    public TService? GetService<TService>() => _serviceProvider.GetService<TService>();

    public TService GetRequiredService<TService>()
        where TService : notnull
        => Services.BuildServiceProvider().GetRequiredService<TService>();
}
