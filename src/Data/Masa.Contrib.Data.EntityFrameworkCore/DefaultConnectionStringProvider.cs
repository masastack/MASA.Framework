namespace Masa.Contrib.Data.EntityFrameworkCore;

public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly IOptionsMonitor<MasaDbConnectionOptions> _options;

    public DefaultConnectionStringProvider(IOptionsMonitor<MasaDbConnectionOptions> options) => _options = options;

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME) => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (string.IsNullOrEmpty(name))
            return _options.CurrentValue.ConnectionStrings.DefaultConnection;

        return _options.CurrentValue.ConnectionStrings.GetConnectionString(name);
    }
}
