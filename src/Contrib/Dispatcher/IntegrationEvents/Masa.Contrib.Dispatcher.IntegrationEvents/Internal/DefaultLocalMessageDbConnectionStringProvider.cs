// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

internal class DefaultLocalMessageDbConnectionStringProvider :
    LocalMessageDbConnectionStringProviderBase,
    ILocalMessageDbConnectionStringProviderWrapper
{
    private readonly IOptionsSnapshot<ConnectionStrings> _options;
    private readonly IOptionsSnapshot<LocalMessageTableOptions> _localMessageTableOptions;

    public DefaultLocalMessageDbConnectionStringProvider(
        IOptionsSnapshot<ConnectionStrings> options,
        IOptionsSnapshot<LocalMessageTableOptions> localMessageTableOptions)
    {
        _options = options;
        _localMessageTableOptions = localMessageTableOptions;
    }

    protected override List<string> GetConnectionStrings()
    {
        if (_localMessageTableOptions.Value.DbContextType == null)
            return new();

        var list = _options
            .Value
            .Where(option => option.Key.Equals(_localMessageTableOptions.Value.SectionName, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.Value)
            .ToList();
        return list;
    }
}
