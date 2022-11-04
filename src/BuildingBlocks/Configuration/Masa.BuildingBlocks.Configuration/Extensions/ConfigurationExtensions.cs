// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigurationExtensions
{
    public static Dictionary<string, string> ConvertToDictionary(this IConfiguration configuration)
    {
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        GetData(configuration, configuration.GetChildren(), ref data);
        return data;
    }

    private static void GetData(
        IConfiguration configuration,
        IEnumerable<IConfigurationSection> configurationSections,
        ref Dictionary<string, string> dictionary)
    {
        foreach (var configurationSection in configurationSections)
        {
            var section = configuration.GetSection(configurationSection.Path);

            var childrenSections = section.GetChildren()?.ToList() ?? new List<IConfigurationSection>();

            if (!section.Exists() || !childrenSections.Any())
            {
                var key = section.Path;
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, configuration[section.Path]);
                }
            }
            else
            {
                GetData(configuration, childrenSections, ref dictionary);
            }
        }
    }
}
