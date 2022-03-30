namespace Masa.Contrib.Isolation.UoW.EF;

public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;
    private readonly IOptionsSnapshot<IsolationDbConnectionOptions> _options;
    private readonly IEnvironmentContext? _environmentContext;
    private readonly ITenantContext? _tenantContext;
    private readonly ILogger<DefaultConnectionStringProvider>? _logger;

    public DefaultConnectionStringProvider(
        IUnitOfWorkAccessor unitOfWorkAccessor,
        IOptionsSnapshot<IsolationDbConnectionOptions> options,
        IEnvironmentContext? environmentContext = null,
        ITenantContext? tenantContext = null,
        ILogger<DefaultConnectionStringProvider>? logger = null)
    {
        _unitOfWorkAccessor = unitOfWorkAccessor;
        _options = options;
        _environmentContext = environmentContext;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public Task<string> GetConnectionStringAsync() => Task.FromResult(GetConnectionString());

    public string GetConnectionString()
    {
        if (_unitOfWorkAccessor.CurrentDbContextOptions != null)
            return _unitOfWorkAccessor.CurrentDbContextOptions.ConnectionString;

        Expression<Func<DbConnectionOptions, bool>> condition = option => true;

        if (_tenantContext != null)
        {
            if (_tenantContext.CurrentTenant == null)
            {
                _logger?.LogError($"Tenant resolution failed, the currently used ConnectionString is [{nameof(_options.Value.DefaultConnection)}]");
                return SetConnectionString();
            }

            condition = condition.And(option => option.TenantId == "*" || (_tenantContext.CurrentTenant!=null && _tenantContext.CurrentTenant.Id.Equals(option.TenantId, StringComparison.CurrentCultureIgnoreCase)));
        }

        if (_environmentContext != null)
        {
            if (string.IsNullOrEmpty(_environmentContext.CurrentEnvironment))
            {
                _logger?.LogError($"Environment resolution failed, the currently used ConnectionString is [{nameof(_options.Value.DefaultConnection)}]");
                return SetConnectionString();
            }

            condition = condition.And(option => option.Environment == "*" || option.Environment.Equals(_environmentContext.CurrentEnvironment, StringComparison.CurrentCultureIgnoreCase));
        }

        string connectionString;
        var list = _options.Value.Isolations.Where(condition.Compile()).ToList();
        if (list.Count >= 1)
        {
            connectionString = list.OrderByDescending(option=>option.Score).Select(option => option.ConnectionString).FirstOrDefault()!;
            if (list.Count > 1)
                _logger?.LogInformation($"{GetMessage()}, Matches multiple available database link strings, the currently used ConnectionString is [{connectionString}]");
        }
        else
        {
            connectionString = _options.Value.DefaultConnection;
            _logger?.LogDebug($"{GetMessage()}, the currently used ConnectionString is [{nameof(_options.Value.DefaultConnection)}]");
        }
        return SetConnectionString(connectionString);
    }

    private string SetConnectionString(string? connectionString = null)
    {
        _unitOfWorkAccessor.CurrentDbContextOptions = new MasaDbContextConfigurationOptions(connectionString ?? _options.Value.DefaultConnection);
        return _unitOfWorkAccessor.CurrentDbContextOptions.ConnectionString;
    }

    private string GetMessage()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (_environmentContext != null)
            stringBuilder.Append($"Environment: [{_environmentContext.CurrentEnvironment}], ");
        if (_tenantContext != null)
            stringBuilder.Append($"Tenant: [{_tenantContext.CurrentTenant?.Id ?? ""}]");
        var message = stringBuilder.ToString();
        return message.Substring(0, message.Length - 1);
    }
}
