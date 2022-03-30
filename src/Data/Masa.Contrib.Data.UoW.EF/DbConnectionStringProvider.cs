namespace Masa.Contrib.Data.UoW.EF;

public class DbConnectionStringProvider : BaseDbConnectionStringProvider
{
    private readonly IOptionsMonitor<MasaDbConnectionOptions> _options;

    public DbConnectionStringProvider(IOptionsMonitor<MasaDbConnectionOptions> options) => _options = options;

    protected override List<DbContextOptions> GetDbContextOptionsList()
    {
        return new() { new(_options.CurrentValue.DefaultConnection) };
    }
}
