// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class DefaultMasaConfigurationSourceProvider : IMasaConfigurationSourceProvider
{
    public virtual (List<IConfigurationSource> MigrateConfigurationSources, List<IConfigurationSource> ConfigurationSources) GetMigrated(
        IConfigurationBuilder configurationBuilder,
        List<Type> excludeConfigurationSourceTypes,
        List<Type> excludeConfigurationProviderTypes)
    {
        List<IConfigurationSource> migrateConfigurationSources = new();
        List<IConfigurationSource> configurationSources = new();
        foreach (var source in configurationBuilder.Sources)
        {
            var result = GetMigrated(source, excludeConfigurationSourceTypes, excludeConfigurationProviderTypes);
            migrateConfigurationSources.AddRange(result.MigrateConfigurationSources);
            configurationSources.AddRange(result.ConfigurationSources);
        }
        return (migrateConfigurationSources, configurationSources);
    }

    public virtual (List<IConfigurationSource> MigrateConfigurationSources, List<IConfigurationSource> ConfigurationSources)
        GetMigrated(
            IConfiguration configuration,
            List<Type> excludeConfigurationSourceTypes,
            List<Type> excludeConfigurationProviderTypes)
    {
        List<IConfigurationSource> migrateConfigurationSources = new();
        List<IConfigurationSource> configurationSources = new();
        if (configuration is IConfigurationBuilder configurationBuilder)
        {
            foreach (var configurationSource in configurationBuilder.Sources)
            {
                var result = GetMigrated(
                    configurationSource,
                    excludeConfigurationSourceTypes,
                    excludeConfigurationProviderTypes);
                migrateConfigurationSources.AddRange(result.MigrateConfigurationSources);
                configurationSources.AddRange(result.ConfigurationSources);
            }
        }
        else if (configuration is IConfigurationRoot configurationRoot)
        {
            foreach (var configurationProvider in configurationRoot.Providers)
            {
                var masaConfigurationSource = new MasaConfigurationSource(configurationProvider);
                if (excludeConfigurationProviderTypes.Contains(configurationProvider.GetType()))
                    configurationSources.Add(masaConfigurationSource);
                else migrateConfigurationSources.Add(masaConfigurationSource);
            }
        }
        return new(migrateConfigurationSources, configurationSources);
    }

    public virtual (List<IConfigurationSource> MigrateConfigurationSources, List<IConfigurationSource> ConfigurationSources)
        GetMigrated(
            IConfigurationSource configurationSource,
            List<Type> excludeConfigurationSourceTypes,
            List<Type> excludeConfigurationProviderTypes)
    {
        List<IConfigurationSource> migrateConfigurationSources = new();
        List<IConfigurationSource> configurationSources = new();
        if (excludeConfigurationSourceTypes.Contains(configurationSource.GetType()))
        {
            configurationSources.Add(configurationSource);
            return (migrateConfigurationSources, configurationSources);
        }
        if (configurationSource is ChainedConfigurationSource chainedConfigurationSource)
            return GetMigrated(
                chainedConfigurationSource.Configuration,
                excludeConfigurationSourceTypes,
                excludeConfigurationProviderTypes);

        migrateConfigurationSources.Add(configurationSource);
        return (migrateConfigurationSources, configurationSources);
    }
}
