namespace MASA.Contrib.Data.Uow.EF;

public class DispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
