// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public interface IMasaConfigurationSourceProvider
{
    (List<IConfigurationSource> MigrateConfigurationSources, List<IConfigurationSource> ConfigurationSources) GetMigrated(
        IEnumerable<IConfigurationSource> originalConfigurationSources,
        List<Type> excludeConfigurationSourceTypes,
        List<Type> excludeConfigurationProviderTypes,
        List<MigrateConfigurationRelationsInfo> migrateRelations);
}
