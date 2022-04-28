// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EF;

public class DbConnectionStringProvider : BaseDbConnectionStringProvider
{
    private readonly IOptionsMonitor<MasaDbConnectionOptions> _options;

    public DbConnectionStringProvider(IOptionsMonitor<MasaDbConnectionOptions> options) => _options = options;

    protected override List<DbContextOptions> GetDbContextOptionsList()
    {
        return new() { new(_options.CurrentValue.DefaultConnection) };
    }
}
