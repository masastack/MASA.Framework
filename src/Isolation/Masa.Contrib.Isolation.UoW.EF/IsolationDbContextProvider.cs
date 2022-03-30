namespace Masa.Contrib.Isolation.UoW.EF;

public class IsolationDbContextProvider : BaseDbConnectionStringProvider
{
    private readonly IOptionsMonitor<IsolationDbConnectionOptions> _options;

    public IsolationDbContextProvider(IOptionsMonitor<IsolationDbConnectionOptions> options) => _options = options;

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
    {
        var connectionStrings = _options.CurrentValue.Isolations
            .Select(connectionString => connectionString.ConnectionString)
            .Distinct()
            .ToList();
        if (!connectionStrings.Contains(_options.CurrentValue.DefaultConnection))
            connectionStrings.Add(_options.CurrentValue.DefaultConnection);

        return connectionStrings.Select(connectionString => new MasaDbContextConfigurationOptions(connectionString)).ToList();
    }
}
