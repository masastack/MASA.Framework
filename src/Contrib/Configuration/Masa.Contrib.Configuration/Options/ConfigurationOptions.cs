// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

public class ConfigurationOptions
{
    public IEnumerable<Assembly> Assemblies { get; set; }

    public List<Type> ExcludeConfigurationSourceTypes { get; set; }

    public List<Type> ExcludeConfigurationProviderTypes { get; set; }

    /// <summary>
    /// Mapping relationship, specifying the relationship between the configuration node and the target configuration node
    /// For example: Logging -> Local:Logging
    /// </summary>
    public List<MigrateConfigurationRelationsInfo> MigrateRelations { get; set; }

    public ConfigurationOptions()
    {
        Assemblies = MasaApp.GetAssemblies();
        ExcludeConfigurationSourceTypes = Internal.ConfigurationExtensions.DefaultExcludeConfigurationSourceTypes;
        ExcludeConfigurationProviderTypes = Internal.ConfigurationExtensions.DefaultExcludeConfigurationProviderTypes;
        InitializeMigrateRelations();
    }

    void InitializeMigrateRelations()
    {
        MigrateRelations = new();
        AddLoggingMigrateRelations();
    }

    void AddLoggingMigrateRelations()
    {
        MigrateRelations.Add(new(ConfigurationConstant.LOGGING_SECTION,
            $"{nameof(SectionTypes.Local)}{ConfigurationPath.KeyDelimiter}{ConfigurationConstant.LOGGING_SECTION}"));
    }
}
