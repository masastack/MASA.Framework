// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

internal class DefaultConnectionStringProvider : IConnectionStringProviderWrapper
{
    private readonly IOptionsSnapshot<MasaDbConnectionOptions> _options;

    public DefaultConnectionStringProvider(IOptionsSnapshot<MasaDbConnectionOptions> options) => _options = options;

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
        => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (string.IsNullOrEmpty(name))
            return _options.Value.ConnectionStrings.DefaultConnection;

        return _options.Value.ConnectionStrings.GetConnectionString(name);
    }
}

