// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Internal;

internal static class ConfigurationExtensions
{
    public static readonly List<Type> DefaultExcludeConfigurationSourceTypes = new()
    {
        typeof(CommandLineConfigurationSource),
        typeof(EnvironmentVariablesConfigurationSource),
        typeof(KeyPerFileConfigurationSource),
        typeof(MemoryConfigurationSource)
    };

    public static IConfigurationBuilder AddRange(this IConfigurationBuilder configurationBuilder,
        IEnumerable<IConfigurationSource> configurationSources)
    {
        foreach (var configurationSource in configurationSources)
        {
            configurationBuilder.Add(configurationSource);
        }
        return configurationBuilder;
    }
}
