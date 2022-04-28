// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EF;

public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly IIsolationDbConnectionStringProvider _isolationConnectionStringProvider;

    public DefaultConnectionStringProvider(IIsolationDbConnectionStringProvider isolationConnectionStringProvider)
        => _isolationConnectionStringProvider = isolationConnectionStringProvider;

    public Task<string> GetConnectionStringAsync() => _isolationConnectionStringProvider.GetConnectionStringAsync();

    public string GetConnectionString() => _isolationConnectionStringProvider.GetConnectionString();
}
