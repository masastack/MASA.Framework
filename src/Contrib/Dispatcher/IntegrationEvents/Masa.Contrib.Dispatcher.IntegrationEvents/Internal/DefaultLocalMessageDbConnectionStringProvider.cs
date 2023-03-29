// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

internal class DefaultLocalMessageDbConnectionStringProvider :
    LocalMessageDbConnectionStringProviderBase,
    ILocalMessageDbConnectionStringProviderWrapper
{
    private readonly IConnectionStringConfigProvider? _localConnectionStringProvider;
    private readonly IOptionsSnapshot<LocalMessageTableOptions> _localMessageTableOptions;

    public DefaultLocalMessageDbConnectionStringProvider(
        IOptionsSnapshot<LocalMessageTableOptions> localMessageTableOptions,
        IConnectionStringConfigProvider? localConnectionStringProvider = null)
    {
        _localConnectionStringProvider = localConnectionStringProvider;
        _localMessageTableOptions = localMessageTableOptions;
    }

    protected override List<string> GetConnectionStrings()
    {
        if (_localMessageTableOptions.Value.DbContextType == null)
            return new();

        var list = _localConnectionStringProvider?
            .GetConnectionStrings()
            .Where(option => option.Key.Equals(_localMessageTableOptions.Value.SectionName, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.Value)
            .ToList() ?? new List<string>();
        return list;
    }
}
