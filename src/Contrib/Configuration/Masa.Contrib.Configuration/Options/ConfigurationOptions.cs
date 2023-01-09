// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

public class ConfigurationOptions
{
    public IEnumerable<Assembly> Assemblies { get; set; }

    public List<Type> ExcludeConfigurationSourceTypes { get; set; }

    public List<Type> ExcludeConfigurationProviderTypes { get; set; }

    public ConfigurationOptions()
    {
        Assemblies = MasaApp.GetAssemblies();
        ExcludeConfigurationSourceTypes = Internal.ConfigurationExtensions.DefaultExcludeConfigurationSourceTypes;
        ExcludeConfigurationProviderTypes = Internal.ConfigurationExtensions.DefaultExcludeConfigurationProviderTypes;
    }
}
