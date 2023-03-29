// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Isolation.EFCore.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

internal class DefaultIsolationConnectionStringProvider : IIsolationConnectionStringProviderWrapper
{
    private readonly IUnitOfWorkAccessor? _unitOfWorkAccessor;
    private readonly IMultiEnvironmentContext? _environmentContext;
    private readonly IMultiTenantContext? _tenantContext;
    private readonly IConnectionStringProviderWrapper _connectionStringProviderWrapper;
    private readonly ILogger<DefaultIsolationConnectionStringProvider>? _logger;
    private readonly IIsolationConfigProvider _isolationConfigProvider;

    public DefaultIsolationConnectionStringProvider(
        IConnectionStringProviderWrapper connectionStringProviderWrapper,
        IIsolationConfigProvider isolationConfigProvider,
        IUnitOfWorkAccessor? unitOfWorkAccessor = null,
        IMultiEnvironmentContext? environmentContext = null,
        IMultiTenantContext? tenantContext = null,
        ILogger<DefaultIsolationConnectionStringProvider>? logger = null)
    {
        _connectionStringProviderWrapper = connectionStringProviderWrapper;
        _isolationConfigProvider = isolationConfigProvider;
        _unitOfWorkAccessor = unitOfWorkAccessor;
        _environmentContext = environmentContext;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    [ExcludeFromCodeCoverage]
    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
        => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (_unitOfWorkAccessor != null &&
            _unitOfWorkAccessor.CurrentDbContextOptions.TryGetConnectionString(name, out var connectionString))
            return connectionString;

        var connectionStrings =
            _isolationConfigProvider.GetComponentConfig<ConnectionStrings>(ConnectionStrings.DEFAULT_SECTION,
                ConnectionStrings.DEFAULT_SECTION);
        if (connectionStrings != null)
            return SetConnectionString(name, connectionStrings.GetConnectionString(name));

        connectionString = _connectionStringProviderWrapper.GetConnectionString(name);
        _logger?.LogDebug(
            "{Message}, the currently used ConnectionString is [{ConnectionString}]",
            GetMessage(),
            connectionString);

        return SetConnectionString(name, connectionString);
    }

    private string SetConnectionString(string name, string connectionString)
    {
        if (_unitOfWorkAccessor != null)
            _unitOfWorkAccessor.CurrentDbContextOptions.AddConnectionString(name, connectionString);

        return connectionString;
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
