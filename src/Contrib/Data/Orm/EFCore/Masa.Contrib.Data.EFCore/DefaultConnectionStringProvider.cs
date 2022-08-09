// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore;

public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly IOptionsMonitor<MasaDbConnectionOptions> _options;

    public DefaultConnectionStringProvider(IOptionsMonitor<MasaDbConnectionOptions> options) => _options = options;

    public Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME) => Task.FromResult(GetConnectionString(name));

    public string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
    {
        if (string.IsNullOrEmpty(name))
            return _options.CurrentValue.ConnectionStrings.DefaultConnection;

        return _options.CurrentValue.ConnectionStrings.GetConnectionString(name);
    }
}
