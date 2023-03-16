// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

internal class DefaultConnectionStringProvider : IConnectionStringProviderWrapper
{
    private readonly IOptionsSnapshot<ConnectionStrings> _options;

    public DefaultConnectionStringProvider(IOptionsSnapshot<ConnectionStrings> options) => _options = options;

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
        => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (string.IsNullOrEmpty(name))
            return _options.Value.DefaultConnection;

        return _options.Value.GetConnectionString(name);
    }
}

