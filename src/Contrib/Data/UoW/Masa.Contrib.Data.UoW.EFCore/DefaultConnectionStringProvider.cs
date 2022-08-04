// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore;

public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;
    private readonly IOptionsMonitor<MasaDbConnectionOptions> _options;
    private readonly ILogger<DefaultConnectionStringProvider>? _logger;

    public DefaultConnectionStringProvider(
        IUnitOfWorkAccessor unitOfWorkAccessor,
        IOptionsMonitor<MasaDbConnectionOptions> options,
        ILogger<DefaultConnectionStringProvider>? logger = null)
    {
        _unitOfWorkAccessor = unitOfWorkAccessor;
        _options = options;
        _logger = logger;
    }

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME) => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (_unitOfWorkAccessor.CurrentDbContextOptions != null)
            return _unitOfWorkAccessor.CurrentDbContextOptions.ConnectionString;

        var connectionStrings = _options.CurrentValue.ConnectionStrings;
        var connectionString = string.IsNullOrEmpty(name) ? connectionStrings.DefaultConnection : connectionStrings.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
            _logger?.LogError("Failed to get database connection string, please check whether the configuration of IOptionsSnapshot<MasaDbConnectionOptions> is abnormal");

        _unitOfWorkAccessor.CurrentDbContextOptions = new MasaDbContextConfigurationOptions(connectionString);
        return connectionString;
    }
}
