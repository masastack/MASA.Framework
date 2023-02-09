// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation;

public class IsolationDbContextProvider : DbConnectionStringProviderBase
{
    private readonly IOptionsSnapshot<IsolationDbConnectionOptions> _options;

    public IsolationDbContextProvider(IOptionsSnapshot<IsolationDbConnectionOptions> options) => _options = options;

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
    {
        var connectionStrings = _options.Value.IsolationConnectionStrings
            .Select(connectionString => connectionString.ConnectionString)
            .Distinct()
            .ToList();
        if (!connectionStrings.Contains(_options.Value.ConnectionStrings.DefaultConnection))
            connectionStrings.Add(_options.Value.ConnectionStrings.DefaultConnection);

        return connectionStrings.Select(connectionString => new MasaDbContextConfigurationOptions(connectionString)).ToList();
    }
}
