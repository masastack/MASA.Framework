namespace Masa.Contrib.Data.UoW.EF;

public class DataConnectionStringProvider : BaseDataConnectionStringProvider
{
    private readonly IOptionsMonitor<MasaDbConnectionOptions> _options;

    public DataConnectionStringProvider(IOptionsMonitor<MasaDbConnectionOptions> options)
    {
        _options = options;
    }

    protected override List<DbContextOptions> GetDbContextOptionsList()
    {
        return new ()
        {
            new(_options.CurrentValue.DefaultConnection)
        };
    }
}
