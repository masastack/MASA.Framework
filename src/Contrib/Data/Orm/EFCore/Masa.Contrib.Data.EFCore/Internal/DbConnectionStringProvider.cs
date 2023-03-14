// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

internal class DbConnectionStringProvider : DbConnectionStringProviderBase, IDbConnectionStringProviderWrapper
{
    private readonly IOptionsSnapshot<MasaDbConnectionOptions> _options;
    private readonly IOptionsSnapshot<LocalMessageTableOptions> _localMessageTableOptions;

    public DbConnectionStringProvider(
        IOptionsSnapshot<MasaDbConnectionOptions> options,
        IOptionsSnapshot<LocalMessageTableOptions> localMessageTableOptions)
    {
        _options = options;
        _localMessageTableOptions = localMessageTableOptions;
    }

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
    {
        var list = _options
            .Value
            .ConnectionStrings
            .Where(option => option.Key.Equals(_localMessageTableOptions.Value.SectionName, StringComparison.OrdinalIgnoreCase))
            .Select(item => new MasaDbContextConfigurationOptions(item.Value))
            .Distinct()
            .ToList();
        return list;
    }
}
