// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class DbConnectionStringProvider : DbConnectionStringProviderBase
{
    private readonly IOptionsSnapshot<MasaDbConnectionOptions> _options;

    public DbConnectionStringProvider(IOptionsSnapshot<MasaDbConnectionOptions> options) => _options = options;

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
        => _options.Value.ConnectionStrings.Select(item => new MasaDbContextConfigurationOptions(item.Value)).Distinct().ToList();
}
