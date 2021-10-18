namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests.Infrastructure.Options;

public class DispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
