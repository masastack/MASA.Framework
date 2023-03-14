// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

public class IsolationDbConnectionStringProvider : DbConnectionStringProviderBase, IIsolationDbConnectionStringProviderWrapper
{
    private readonly IOptionsSnapshot<IsolationOptions> _options;
    private readonly IDbConnectionStringProviderWrapper _dbConnectionStringProviderWrapper;
    private readonly IOptionsSnapshot<LocalMessageTableOptions> _localMessageTableOptions;

    public IsolationDbConnectionStringProvider(
        IOptionsSnapshot<IsolationOptions> options,
        IDbConnectionStringProviderWrapper dbConnectionStringProviderWrapper,
        IOptionsSnapshot<LocalMessageTableOptions> localMessageTableOptions)
    {
        _options = options;
        _dbConnectionStringProviderWrapper = dbConnectionStringProviderWrapper;
        _localMessageTableOptions = localMessageTableOptions;
    }

    protected override List<MasaDbContextConfigurationOptions> GetDbContextOptionsList()
    {
        if (_localMessageTableOptions.Value.SectionName.IsNullOrWhiteSpace())
            return new();

        var masaDbContextConfigurationOptions =
            new List<MasaDbContextConfigurationOptions>(_dbConnectionStringProviderWrapper.DbContextOptionsList);
        var connectionStrings = GetDbConnectionStringByIsolation();
        foreach (var connectionString in connectionStrings)
        {
            if (masaDbContextConfigurationOptions.Any(option => option.ConnectionString == connectionString))
                continue;

            masaDbContextConfigurationOptions.Add(new MasaDbContextConfigurationOptions(connectionString));
        }
        return masaDbContextConfigurationOptions;
    }

    private List<string> GetDbConnectionStringByIsolation()
    {
        var modules = _options.Value.Data.Select(option => option.Module).ToList();

        List<string> connectionStrings = new();
        foreach (var module in modules)
        {
            if (ModuleConfigUtils.TryGetConfig<MasaDbConnectionOptions>(
                    module,
                    ConnectionStrings.DEFAULT_SECTION,
                    out var dbConnectionOptions))
            {
                var connectionString =
                    dbConnectionOptions.ConnectionStrings.GetConnectionString(_localMessageTableOptions.Value.SectionName);
                if (!connectionString.IsNullOrWhiteSpace())
                    connectionStrings.Add(connectionString);
            }
        }
        return connectionStrings;
    }
}
