namespace MASA.Contribs.DDD.Domain.Repository.EF;

public class DispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
