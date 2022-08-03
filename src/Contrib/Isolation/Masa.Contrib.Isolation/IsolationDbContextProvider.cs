// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation;

public class IsolationDbContextProvider : BaseDbConnectionStringProvider
{
    private readonly IOptionsMonitor<IsolationDbConnectionOptions> _options;

    public IsolationDbContextProvider(IOptionsMonitor<IsolationDbConnectionOptions> options) => _options = options;

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
    {
        var connectionStrings = _options.CurrentValue.IsolationConnectionStrings
            .Select(connectionString => connectionString.ConnectionString)
            .Distinct()
            .ToList();
        if (!connectionStrings.Contains(_options.CurrentValue.ConnectionStrings.DefaultConnection))
            connectionStrings.Add(_options.CurrentValue.ConnectionStrings.DefaultConnection);

        return connectionStrings.Select(connectionString => new MasaDbContextConfigurationOptions(connectionString)).ToList();
    }
}
