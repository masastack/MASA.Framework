// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

internal class DefaultLocalMessageDbConnectionStringProvider :
    LocalMessageDbConnectionStringProviderBase,
    ILocalMessageDbConnectionStringProviderWrapper
{
    private readonly IOptionsSnapshot<MasaDbConnectionOptions> _options;
    private readonly IOptionsSnapshot<LocalMessageTableOptions> _localMessageTableOptions;

    public DefaultLocalMessageDbConnectionStringProvider(
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
