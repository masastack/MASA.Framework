namespace Masa.Contrib.Configuration;

public class MasaConfigurationBuilder : IMasaConfigurationBuilder
{
    private readonly IConfigurationBuilder _builder;

    public IDictionary<string, object> Properties => _builder.Properties;

    public IList<IConfigurationSource> Sources => _builder.Sources;

    public Dictionary<string, IConfiguration> GetSectionRelations() => SectionRelations;

    internal Dictionary<string, IConfiguration> SectionRelations { get; } = new();

    internal List<IConfigurationRepository> Repositories { get; } = new();

    internal List<ConfigurationRelationOptions> Relations { get; } = new();

    public MasaConfigurationBuilder(IConfigurationBuilder builder)
        => _builder = builder;

    /// <summary>
    ///
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="sectionName">If section is null, it is mounted to the Local section</param>
    public void AddSection(IConfigurationBuilder configurationBuilder, string? sectionName = null)
    {
        if (configurationBuilder == null)
            throw new ArgumentNullException(nameof(configurationBuilder));

        if (configurationBuilder.Sources.Count == 0)
            throw new ArgumentException("Source cannot be empty");

        sectionName = sectionName ?? "";

        if (SectionRelations.ContainsKey(sectionName))
            throw new ArgumentException("Section already exists", nameof(sectionName));

        SectionRelations.Add(sectionName, configurationBuilder.Build());
    }

    public void AddRepository(IConfigurationRepository configurationRepository)
        => Repositories.Add(configurationRepository);

    public void AddRelations(params ConfigurationRelationOptions[] relationOptions)
        => Relations.AddRange(relationOptions);

    public IConfigurationBuilder Add(IConfigurationSource source) => _builder.Add(source);

    public IConfigurationRoot Build() => _builder.Build();
}
