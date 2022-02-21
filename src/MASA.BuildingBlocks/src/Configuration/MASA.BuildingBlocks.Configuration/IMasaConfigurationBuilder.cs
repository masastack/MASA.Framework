namespace MASA.BuildingBlocks.Configuration;
public interface IMasaConfigurationBuilder : IConfigurationBuilder
{
    Dictionary<string, IConfiguration> GetSectionRelations();

    /// <summary>
    /// Mount node information
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="sectionName">The default is the root node</param>
    void AddSection(IConfigurationBuilder configurationBuilder, string? sectionName = null);

    void AddRepository(IConfigurationRepository configurationRepository);

    void AddRelations(params ConfigurationRelationOptions[] relationOptions);
}
