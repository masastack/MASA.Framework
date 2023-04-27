// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

public class MasaConfigurationRelationOptions
{
    internal List<ConfigurationRelationOptions> Relations { get; } = new();

    /// <summary>
    /// Map Section relationship By Local
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="section">The default is null, which is consistent with the mapping class name, and string.Empty when no root node exists</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public MasaConfigurationRelationOptions MappingLocal<TModel>(string? section = null, string? name = null) where TModel : class
        => Mapping<TModel>(SectionTypes.Local, null!, section, name);

    /// <summary>
    /// Map Section relationship By ConfigurationApi
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="appId">Dcc's appId</param>
    /// <param name="section">The default is null, which is consistent with the mapping class name, and string.Empty when no root node exists</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public MasaConfigurationRelationOptions MappingConfigurationApi<TModel>(string appId, string? section = null, string? name = null)
        where TModel : class
        => Mapping<TModel>(SectionTypes.ConfigurationApi, appId, section, name);

    /// <summary>
    /// Map Section relationship
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="sectionType"></param>
    /// <param name="parentSection">parent section, local section is the name of the locally configured section, and ConfigurationApi is the name of the Appid where the configuration is located</param>
    /// <param name="section">The default is null, which is consistent with the mapping class name</param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private MasaConfigurationRelationOptions Mapping<TModel>(SectionTypes sectionType, string parentSection, string? section = null, string? name = null)
        where TModel : class
    {
        name ??= Options.DefaultName;
        section ??= typeof(TModel).Name;

        if (Relations.Any(relation => relation.SectionType == sectionType && relation.Section == section && relation.Name == name))
            throw new ArgumentOutOfRangeException(nameof(section), "The current section already has a configuration");

        Relations.Add(new ConfigurationRelationOptions()
        {
            SectionType = sectionType,
            ParentSection = parentSection,
            Section = section,
            ObjectType = typeof(TModel),
            Name = name
        });
        return this;
    }
}
