namespace Masa.Contrib.Isolation;

public class IsolationBuilder : IIsolationBuilder
{
    public IServiceCollection Services { get; }

    public IsolationBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
