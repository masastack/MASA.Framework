using Masa.BuildingBlocks.Data;

namespace Masa.Contrib.Data.EntityFrameworkCore;

public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly IOptionsSnapshot<MasaDbConnectionOptions> _options;

    public DefaultConnectionStringProvider(IOptionsSnapshot<MasaDbConnectionOptions> options) => _options = options;

    public Task<string> GetConnectionStringAsync() => Task.FromResult(GetConnectionString());

    public string GetConnectionString() => _options.Value.DefaultConnection;
}
