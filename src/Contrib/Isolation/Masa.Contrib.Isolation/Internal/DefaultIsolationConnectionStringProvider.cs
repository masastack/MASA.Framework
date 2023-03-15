// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Isolation;

internal class DefaultIsolationConnectionStringProvider : IIsolationConnectionStringProviderWrapper
{
    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;
    private readonly IMultiEnvironmentContext? _environmentContext;
    private readonly IMultiTenantContext? _tenantContext;
    private readonly IConnectionStringProviderWrapper _connectionStringProviderWrapper;
    private readonly ILogger<DefaultIsolationConnectionStringProvider>? _logger;
    private readonly IIsolationConfigurationProvider<MasaDbConnectionOptions> _configurationProvider;

    public DefaultIsolationConnectionStringProvider(
        IUnitOfWorkAccessor unitOfWorkAccessor,
        IConnectionStringProviderWrapper connectionStringProviderWrapper,
        IIsolationConfigurationProvider<MasaDbConnectionOptions> configurationProvider,
        IMultiEnvironmentContext? environmentContext = null,
        IMultiTenantContext? tenantContext = null,
        ILogger<DefaultIsolationConnectionStringProvider>? logger = null)
    {
        _unitOfWorkAccessor = unitOfWorkAccessor;
        _configurationProvider = configurationProvider;
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
            return _unitOfWorkAccessor.CurrentDbContextOptions.ConnectionString; //todo: UnitOfWork does not currently support multi-context versions

        if (_configurationProvider.TryGetModule(name, out var masaDbConnectionOptions))
            return SetConnectionString(masaDbConnectionOptions.ConnectionStrings.GetConnectionString(name));

        var connectionString = _connectionStringProviderWrapper.GetConnectionString(name);
        _logger?.LogDebug(
            "{Message}, the currently used ConnectionString is [{ConnectionString}]",
            GetMessage(),
            connectionString);

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
