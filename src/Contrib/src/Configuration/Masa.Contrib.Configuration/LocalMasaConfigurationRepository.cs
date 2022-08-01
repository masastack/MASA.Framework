// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

internal class LocalMasaConfigurationRepository : AbstractConfigurationRepository
{
    public override SectionTypes SectionType => SectionTypes.Local;

    private Properties _data = new();

    public LocalMasaConfigurationRepository(
        IConfiguration configuration,
        ILoggerFactory? loggerFactory)
        : base(loggerFactory)
    {
        Initialize(configuration);

        ChangeToken.OnChange(configuration.GetReloadToken, () =>
        {
            Initialize(configuration);
            FireRepositoryChange(SectionType, Load());
        });
    }

    private void Initialize(IConfiguration configuration)
    {
        Dictionary<string, string> data = new();
        GetData(configuration, configuration.GetChildren(), ref data);
        _data = new Properties(data);
    }

    private void GetData(IConfiguration configuration, IEnumerable<IConfigurationSection> configurationSections,
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

    public override Properties Load()
    {
        return _data;
    }
}
