namespace Masa.Contrib.Data.UoW.EF;

public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;
    private readonly IOptionsSnapshot<MasaDbConnectionOptions> _options;
    private readonly ILogger<DefaultConnectionStringProvider>? _logger;

    public DefaultConnectionStringProvider(
        IUnitOfWorkAccessor unitOfWorkAccessor,
        IOptionsSnapshot<MasaDbConnectionOptions> options,
        ILogger<DefaultConnectionStringProvider>? logger = null)
    {
        _unitOfWorkAccessor = unitOfWorkAccessor;
        _options = options;
        _logger = logger;
    }

    public Task<string> GetConnectionStringAsync() => Task.FromResult(GetConnectionString());

    public string GetConnectionString()
    {
        if (_unitOfWorkAccessor.CurrentDbContextOptions != null)
            return _unitOfWorkAccessor.CurrentDbContextOptions.ConnectionString;

        var connectionString = _options.Value.DefaultConnection;
        if (string.IsNullOrEmpty(connectionString))
            _logger?.LogError("Failed to get database connection string, please check whether the configuration of IOptionsSnapshot<MasaDbConnectionOptions> is abnormal");

        _unitOfWorkAccessor.CurrentDbContextOptions = new MasaDbContextConfigurationOptions(connectionString);
        return connectionString;
    }
}
