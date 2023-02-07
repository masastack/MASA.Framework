// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class DbConnectionStringProvider : DbConnectionStringProviderBase
{
    private readonly IOptionsMonitor<MasaDbConnectionOptions> _options;

    public DbConnectionStringProvider(IOptionsMonitor<MasaDbConnectionOptions> options) => _options = options;

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
        => _options.CurrentValue.ConnectionStrings.Select(item => new MasaDbContextConfigurationOptions(item.Value)).Distinct().ToList();
}
