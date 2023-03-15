// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Isolation;

internal class DefaultIsolationLocalMessageDbConnectionStringProvider :
    LocalMessageDbConnectionStringProviderBase,
    IIsolationLocalMessageDbConnectionStringProviderWrapper
{
    private readonly ILocalMessageDbConnectionStringProviderWrapper _localMessageDbConnectionStringProviderWrapper;
    private readonly IIsolationConfigurationProvider<MasaDbConnectionOptions> _configurationProvider;
    private readonly IOptionsSnapshot<LocalMessageTableOptions> _localMessageTableOptions;

    public DefaultIsolationLocalMessageDbConnectionStringProvider(
        ILocalMessageDbConnectionStringProviderWrapper localMessageDbConnectionStringProviderWrapper,
        IIsolationConfigurationProvider<MasaDbConnectionOptions> configurationProvider,
        IOptionsSnapshot<LocalMessageTableOptions> localMessageTableOptions)
    {
        _localMessageDbConnectionStringProviderWrapper = localMessageDbConnectionStringProviderWrapper;
        _configurationProvider = configurationProvider;
        _localMessageTableOptions = localMessageTableOptions;
    }

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
    {
        if (_localMessageTableOptions.Value.SectionName.IsNullOrWhiteSpace())
            return new();

        var masaDbContextConfigurationOptions = new List<MasaDbContextConfigurationOptions>(_localMessageDbConnectionStringProviderWrapper.DbContextOptionsList);
        var connectionString = GetDbConnectionStringByIsolation();
        if (connectionString != null && masaDbContextConfigurationOptions.All(option => option.ConnectionString != connectionString))
        {
            masaDbContextConfigurationOptions.Add(new MasaDbContextConfigurationOptions(connectionString));
        }
        return masaDbContextConfigurationOptions;
    }

    private string? GetDbConnectionStringByIsolation()
    {
        if (_configurationProvider.TryGetModule(ConnectionStrings.DEFAULT_SECTION, out var masaDbConnectionOptions))
        {
            return masaDbConnectionOptions.ConnectionStrings.GetConnectionString(_localMessageTableOptions.Value.SectionName);
        }
        return null;
    }
}
