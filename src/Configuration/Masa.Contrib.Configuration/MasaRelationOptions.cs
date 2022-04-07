namespace Masa.Contrib.Configuration;

public class MasaRelationOptions
{
    internal List<ConfigurationRelationOptions> Relations { get; } = new();

    /// <summary>
    /// Map Section relationship By Local
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="section">The default is null, which is consistent with the mapping class name, and string.Empty when no root node exists</param>
    /// <returns></returns>
    public MasaRelationOptions MappingLocal<TModel>(string? section = null) where TModel : class
        => Mapping<TModel>(SectionTypes.Local, null!, section);

    /// <summary>
    /// Map Section relationship By ConfigurationAPI
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="parentSection">The name of the parent section, if it is empty, it will be mounted under SectionType, otherwise it will be mounted to the specified section under SectionType</param>
    /// <param name="section">The default is null, which is consistent with the mapping class name, and string.Empty when no root node exists</param>
    /// <returns></returns>
    public MasaRelationOptions MappingConfigurationApi<TModel>(string parentSection, string? section = null) where TModel : class
        => Mapping<TModel>(SectionTypes.ConfigurationAPI, parentSection, section);

    /// <summary>
    /// Map Section relationship
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="sectionType"></param>
    /// <param name="parentSection">parent section, local section is the name of the locally configured section, and ConfigurationAPI is the name of the Appid where the configuration is located</param>
    /// <param name="section">The default is null, which is consistent with the mapping class name</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public MasaRelationOptions Mapping<TModel>(SectionTypes sectionType, string parentSection, string? section = null) where TModel : class
    {
        section ??= typeof(TModel).Name;

        if (Relations.Any(relation => relation.SectionType == sectionType && relation.Section == section))
            throw new ArgumentOutOfRangeException(nameof(section), "The current section already has a configuration");

        Relations.Add(new ConfigurationRelationOptions()
        {
            SectionType = sectionType,
            ParentSection = parentSection,
            Section = section,
            ObjectType = typeof(TModel)
        });
        return this;
    }
}
