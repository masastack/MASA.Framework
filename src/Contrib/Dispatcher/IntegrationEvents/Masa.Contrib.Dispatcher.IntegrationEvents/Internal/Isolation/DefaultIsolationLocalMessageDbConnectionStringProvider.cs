// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

internal class DefaultIsolationLocalMessageDbConnectionStringProvider :
    LocalMessageDbConnectionStringProviderBase,
    IIsolationLocalMessageDbConnectionStringProviderWrapper
{
    private readonly ILocalMessageDbConnectionStringProviderWrapper _localMessageDbConnectionStringProviderWrapper;
    private readonly IIsolationConfigurationProvider _configurationProvider;
    private readonly IOptionsSnapshot<LocalMessageTableOptions> _localMessageTableOptions;

    public DefaultIsolationLocalMessageDbConnectionStringProvider(
        ILocalMessageDbConnectionStringProviderWrapper localMessageDbConnectionStringProviderWrapper,
        IIsolationConfigurationProvider configurationProvider,
        IOptionsSnapshot<LocalMessageTableOptions> localMessageTableOptions)
    {
        _localMessageDbConnectionStringProviderWrapper = localMessageDbConnectionStringProviderWrapper;
        _configurationProvider = configurationProvider;
        _localMessageTableOptions = localMessageTableOptions;
    }

    protected override List<string> GetConnectionStrings()
    {
        if (_localMessageTableOptions.Value.SectionName.IsNullOrWhiteSpace())
            return new();

        var localConnectionStrings = _localMessageDbConnectionStringProviderWrapper.ConnectionStrings;
        var connectionStrings = new List<string>(GetDbConnectionStringByIsolation());
        foreach (var connectionString in localConnectionStrings)
        {
            if (!connectionStrings.Contains(connectionString))
            {
                connectionStrings.Add(connectionString);
            }
        }

        return connectionStrings;
    }

    private List<string> GetDbConnectionStringByIsolation()
    {
        var masaDbConnectionOptions = _configurationProvider.GetModuleConfigs<ConnectionStrings>(Masa.BuildingBlocks.Data.ConnectionStrings.DEFAULT_SECTION);
        return masaDbConnectionOptions
            .Select(connectionString => connectionString.GetConnectionString(_localMessageTableOptions.Value.SectionName))
            .Distinct()
            .ToList();
    }
}
