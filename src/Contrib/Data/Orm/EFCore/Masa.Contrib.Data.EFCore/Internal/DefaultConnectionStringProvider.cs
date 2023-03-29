// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

internal class DefaultConnectionStringProvider : IConnectionStringProviderWrapper
{
    private readonly IUnitOfWorkAccessor? _unitOfWorkAccessor;
    private readonly IConnectionStringConfigProvider _localConnectionStringProvider;

    public DefaultConnectionStringProvider(
        IConnectionStringConfigProvider localConnectionStringProvider,
        IUnitOfWorkAccessor? unitOfWorkAccessor = null)
    {
        _localConnectionStringProvider = localConnectionStringProvider;
        _unitOfWorkAccessor = unitOfWorkAccessor;
    }

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
        => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (_unitOfWorkAccessor != null)
            return GetConnectionStringByUnitOfWorkAccessor(name);

        return GetConnectionStringCore(name);
    }

    private string GetConnectionStringByUnitOfWorkAccessor(string name)
    {
        if (_unitOfWorkAccessor!.CurrentDbContextOptions.TryGetConnectionString(name, out string? connectionString))
            return connectionString;

        connectionString = GetConnectionStringCore(name);
        _unitOfWorkAccessor.CurrentDbContextOptions.AddConnectionString(name, connectionString);
        return connectionString;
    }

    private string GetConnectionStringCore(string name)
    {
        var actualName = name.IsNullOrWhiteSpace() ? ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME : name;
        var data = _localConnectionStringProvider.GetConnectionStrings();

        if (data.TryGetValue(actualName, out var connectionString))
            return connectionString;

        return string.Empty;
    }
}
