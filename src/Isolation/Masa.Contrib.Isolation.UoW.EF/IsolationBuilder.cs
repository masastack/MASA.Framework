namespace Masa.Contrib.Isolation.UoW.EF;

public class IsolationBuilder : IIsolationBuilder
{
    public IServiceCollection Services { get; }

    public string EnvironmentKey { get; private set; }

    public string TenantKey { get; private set;}

    private List<ITenantParserProvider> _tenantParsers;

    public IReadOnlyCollection<ITenantParserProvider> TenantParsers => _tenantParsers;

    private List<IEnvironmentParserProvider> _environmentParsers;

    public IReadOnlyCollection<IEnvironmentParserProvider> EnvironmentParsers => _environmentParsers;

    public IsolationBuilder(IServiceCollection services)
    {
        Services = services;
        EnvironmentKey = "ASPNETCORE_ENVIRONMENT";
        TenantKey = "__tenant";
        _tenantParsers = new List<ITenantParserProvider>()
        {
            new HttpContextItemTenantParserProvider(),
            new QueryStringTenantParserProvider(),
            new FormTenantParserProvider(),
            new RouteTenantParserProvider(),
            new HeaderTenantParserProvider(),
            new CookieTenantParserProvider()
        };
        _environmentParsers = new List<IEnvironmentParserProvider>()
        {
            new EnvironmentVariablesParserProvider()
        };
    }

    public IsolationBuilder SetEnvironmentKey(string environmentKey)
    {
        EnvironmentKey = environmentKey;
        return this;
    }

    public IsolationBuilder SetTenantKey(string tenantKey)
    {
        TenantKey = tenantKey;
        return this;
    }

    public IsolationBuilder SetTenantParsers(List<ITenantParserProvider> tenantParserProviders)
    {
        _tenantParsers = tenantParserProviders;
        return this;
    }

    public IsolationBuilder SetEnvironmentParsers(List<IEnvironmentParserProvider> environmentParserProviders)
    {
        _environmentParsers = environmentParserProviders;
        return this;
    }
}
