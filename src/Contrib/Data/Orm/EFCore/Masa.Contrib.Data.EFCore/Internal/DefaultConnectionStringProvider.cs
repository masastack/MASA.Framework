// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

internal class DefaultConnectionStringProvider : IConnectionStringProviderWrapper
{
    private readonly IUnitOfWorkAccessor? _unitOfWorkAccessor;
    private readonly IOptionsSnapshot<ConnectionStrings> _options;

    public DefaultConnectionStringProvider(
        IOptionsSnapshot<ConnectionStrings> options,
        IUnitOfWorkAccessor? unitOfWorkAccessor = null)
    {
        _options = options;
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
        if (string.IsNullOrEmpty(name))
            return _options.Value.DefaultConnection;

        return _options.Value.GetConnectionString(name);
    }
}
