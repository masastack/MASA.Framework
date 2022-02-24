namespace Masa.Contrib.Configuration;

internal class LocalMasaConfigurationRepository : AbstractConfigurationRepository
{
    public override SectionTypes SectionType { get; init; }

    private ConcurrentDictionary<string, Properties> _data = new();

    public LocalMasaConfigurationRepository(
        Dictionary<string, IConfiguration> sectionRelation,
        ILoggerFactory? loggerFactory)
        : base(loggerFactory)
    {
        this.SectionType = SectionTypes.Local;
        foreach (var section in sectionRelation)
        {
            Initialize(section.Key, section.Value);

            ChangeToken.OnChange(() => section.Value.GetReloadToken(), () =>
            {
                Initialize(section.Key, section.Value);
                base.FireRepositoryChange(SectionType, Load());
            });
        }
    }

    private void Initialize(string rootSectionName, IConfiguration configuration)
    {
        Dictionary<string, string> data = new();
        GetData(rootSectionName, configuration, configuration.GetChildren(), ref data);
        var properties = new Properties(data);
        _data[rootSectionName] = properties;
    }

    private void GetData(string rootSectionName, IConfiguration configuration, IEnumerable<IConfigurationSection> configurationSections, ref Dictionary<string, string> dictionary)
    {
        foreach (var configurationSection in configurationSections)
        {
            var section = configuration.GetSection(configurationSection.Path);

            var childrenSections = section.GetChildren();

            if (!section.Exists() || !childrenSections.Any())
            {
                var key = string.IsNullOrEmpty(rootSectionName) ? section.Path : $"{rootSectionName}{ConfigurationPath.KeyDelimiter}{section.Path}";
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, configuration[section.Path]);
                }
            }
            else
            {
                GetData(rootSectionName, configuration, childrenSections, ref dictionary);
            }
        }
    }

    public override Properties Load()
    {
        Dictionary<string, string> localProperties = new();
        foreach (var item in _data)
        {
            foreach (var key in item.Value.GetPropertyNames())
            {
                localProperties[key] = item.Value.GetProperty(key) ?? string.Empty;
            }
        }
        return new Properties(localProperties);
    }
}

