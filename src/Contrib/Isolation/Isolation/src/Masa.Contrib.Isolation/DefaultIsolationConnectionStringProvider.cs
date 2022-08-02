// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation;

public class DefaultDbIsolationConnectionStringProvider : IConnectionStringProvider
{
    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;
    private readonly IOptionsSnapshot<IsolationDbConnectionOptions> _options;
    private readonly IEnvironmentContext? _environmentContext;
    private readonly ITenantContext? _tenantContext;
    private readonly ILogger<DefaultDbIsolationConnectionStringProvider>? _logger;

    public DefaultDbIsolationConnectionStringProvider(
        IUnitOfWorkAccessor unitOfWorkAccessor,
        IOptionsSnapshot<IsolationDbConnectionOptions> options,
        IEnvironmentContext? environmentContext = null,
        ITenantContext? tenantContext = null,
        ILogger<DefaultDbIsolationConnectionStringProvider>? logger = null)
    {
        _unitOfWorkAccessor = unitOfWorkAccessor;
        _options = options;
        _environmentContext = environmentContext;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME) => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (_unitOfWorkAccessor.CurrentDbContextOptions != null)
            return _unitOfWorkAccessor.CurrentDbContextOptions.ConnectionString; //todo: UnitOfWork does not currently support multi-context versions

        Expression<Func<IsolationOptions, bool>> condition = option
            => name != ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME ? option.Name == name : option.Name == name || option.Name == string.Empty;

        if (_tenantContext != null)
        {
            if (_tenantContext.CurrentTenant == null)
                _logger?.LogDebug(
                    $"Tenant resolution failed, the currently used ConnectionString is [{nameof(_options.Value.ConnectionStrings.DefaultConnection)}]");

            condition = condition.And(option => option.TenantId == "*" || (_tenantContext.CurrentTenant != null &&
                _tenantContext.CurrentTenant.Id.Equals(option.TenantId, StringComparison.CurrentCultureIgnoreCase)));
        }

        if (_environmentContext != null)
        {
            if (string.IsNullOrEmpty(_environmentContext.CurrentEnvironment))
            {
                _logger?.LogDebug(
                    $"Environment resolution failed, the currently used ConnectionString is [{nameof(_options.Value.ConnectionStrings.DefaultConnection)}]");
            }

            condition = condition.And(option
                => option.Environment == "*" ||
                option.Environment.Equals(_environmentContext.CurrentEnvironment, StringComparison.CurrentCultureIgnoreCase));
        }

        string? connectionString;
        var list = _options.Value.IsolationConnectionStrings.Where(condition.Compile()).ToList();
        if (list.Count >= 1)
        {
            connectionString = list.OrderByDescending(option => option.Score).Select(option => option.ConnectionString).FirstOrDefault()!;
            if (list.Count > 1)
                _logger?.LogInformation(
                    "{Message}, Matches multiple available database link strings, the currently used ConnectionString is [{ConnectionString}]",
                    GetMessage(), connectionString);
        }
        else
        {
            connectionString = _options.Value.ConnectionStrings.DefaultConnection;
            _logger?.LogDebug("{Message}, the currently used ConnectionString is [{ConnectionString}]", GetMessage(),
                nameof(_options.Value.ConnectionStrings.DefaultConnection));
        }
        return SetConnectionString(connectionString);
    }

    private string SetConnectionString(string? connectionString = null)
    {
        _unitOfWorkAccessor.CurrentDbContextOptions =
            new MasaDbContextConfigurationOptions(connectionString ?? _options.Value.ConnectionStrings.DefaultConnection);
        return _unitOfWorkAccessor.CurrentDbContextOptions.ConnectionString;
    }

    private string GetMessage()
    {
        List<string> messages = new List<string>();
        if (_environmentContext != null)
            messages.Add($"Environment: [{_environmentContext.CurrentEnvironment ?? ""}]");
        if (_tenantContext != null)
            messages.Add($"Tenant: [{_tenantContext.CurrentTenant?.Id ?? ""}]");
        return string.Join(", ", messages);
    }
}
