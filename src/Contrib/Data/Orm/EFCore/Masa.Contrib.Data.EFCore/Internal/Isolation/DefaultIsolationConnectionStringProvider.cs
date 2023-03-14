// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

public class DefaultIsolationConnectionStringProvider : IIsolationConnectionStringProviderWrapper
{
    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;
    private readonly IOptionsSnapshot<IsolationOptions> _options;
    private readonly IMultiEnvironmentContext? _environmentContext;
    private readonly IMultiTenantContext? _tenantContext;
    private readonly IConnectionStringProviderWrapper _connectionStringProviderWrapper;
    private readonly ILogger<DefaultIsolationConnectionStringProvider>? _logger;

    public DefaultIsolationConnectionStringProvider(
        IUnitOfWorkAccessor unitOfWorkAccessor,
        IOptionsSnapshot<IsolationOptions> options,
        IConnectionStringProviderWrapper connectionStringProviderWrapper,
        IMultiEnvironmentContext? environmentContext = null,
        IMultiTenantContext? tenantContext = null,
        ILogger<DefaultIsolationConnectionStringProvider>? logger = null)
    {
        _unitOfWorkAccessor = unitOfWorkAccessor;
        _options = options;
        _connectionStringProviderWrapper = connectionStringProviderWrapper;
        _environmentContext = environmentContext;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
        => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (_unitOfWorkAccessor.CurrentDbContextOptions != null)
            return _unitOfWorkAccessor.CurrentDbContextOptions
                .ConnectionString; //todo: UnitOfWork does not currently support multi-context versions

        Expression<Func<IsolationConfigurationOptions, bool>> condition = option => true;

        if (_tenantContext != null)
        {
            if (_tenantContext.CurrentTenant == null)
                _logger?.LogDebug("Tenant resolution failed");

            condition = condition.And(option => option.TenantId == "*" || (_tenantContext.CurrentTenant != null &&
                _tenantContext.CurrentTenant.Id.Equals(option.TenantId, StringComparison.CurrentCultureIgnoreCase)));
        }

        if (_environmentContext != null)
        {
            if (_environmentContext.CurrentEnvironment.IsNullOrEmpty())
                _logger?.LogDebug("Environment resolution failed");

            condition = condition.And(option
                => option.Environment == "*" ||
                option.Environment.Equals(_environmentContext.CurrentEnvironment, StringComparison.CurrentCultureIgnoreCase));
        }

        string? connectionString = null;
        var list = _options.Value.Data.Where(condition.Compile()).OrderByDescending(option => option.Score).ToList();
        if (list.Count >= 1)
        {
            var data = new List<object>(list);
            foreach (var item in data)
            {
                if (ModuleConfigUtils.TryGetConfig<MasaDbConnectionOptions>(
                        item,
                        ConnectionStrings.DEFAULT_SECTION,
                        out var dbConnectionOptions))
                {
                    connectionString = dbConnectionOptions.ConnectionStrings.GetConnectionString(name);
                }
            }
            if (connectionString.IsNullOrWhiteSpace())
            {
                connectionString = _connectionStringProviderWrapper.GetConnectionString(name);
                _logger?.LogInformation(
                    "{Message}, Matched multiple available configurations, but no matching database connection string was found, the currently used ConnectionString is [{ConnectionString}]",
                    GetMessage(),
                    connectionString);
            }
        }
        else
        {
            connectionString = _connectionStringProviderWrapper.GetConnectionString(name);
            _logger?.LogDebug("{Message}, the currently used ConnectionString is [{ConnectionString}]", GetMessage(),
                connectionString);
        }
        return SetConnectionString(connectionString);
    }

    private string SetConnectionString(string connectionString)
    {
        _unitOfWorkAccessor.CurrentDbContextOptions = new MasaDbContextConfigurationOptions(connectionString);

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
