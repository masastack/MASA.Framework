// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

public class MasaConfigurationRelationOptions
{
    private readonly List<ConfigurationRelationOptions> _relationOptions;

    public MasaConfigurationRelationOptions(List<ConfigurationRelationOptions> relationOptions)
        => _relationOptions = relationOptions;

    /// <summary>
    /// Map Section relationship By Local
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="section">The default is null, which is consistent with the mapping class name, and string.Empty when no root node exists</param>
    /// <param name="optionsName"></param>
    /// <returns></returns>
    public MasaConfigurationRelationOptions MappingLocal<TModel>(string? section = null, string? optionsName = null) where TModel : class
        => Mapping<TModel>(SectionTypes.Local, null!, section, optionsName);

    /// <summary>
    /// Map Section relationship By ConfigurationApi
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="appId">Distributed Configuration Center's appId</param>
    /// <param name="section">The default is null, which is consistent with the mapping class name, and string.Empty when no root node exists</param>
    /// <param name="optionsName"></param>
    /// <returns></returns>
    public MasaConfigurationRelationOptions MappingConfigurationApi<TModel>(string appId, string? section = null, string? optionsName = null)
        where TModel : class
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(appId);
        return Mapping<TModel>(SectionTypes.ConfigurationApi, appId, section, optionsName);
    }

    /// <summary>
    /// Map Section relationship
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="sectionType"></param>
    /// <param name="parentSection">parent section, local section is the name of the locally configured section, and ConfigurationApi is the name of the Appid where the configuration is located</param>
    /// <param name="section">The default is null, which is consistent with the mapping class name</param>
    /// <param name="optionsName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private MasaConfigurationRelationOptions Mapping<TModel>(
        SectionTypes sectionType,
        string parentSection,
        string? section = null,
        string? optionsName = null)
        where TModel : class
    {
        optionsName ??= Options.DefaultName;
        section ??= typeof(TModel).Name;

        ConfigurationUtils.AddRegistrationOptions(_relationOptions, new ConfigurationRelationOptions()
        {
            SectionType = sectionType,
            ParentSection = parentSection,
            Section = section,
            ObjectType = typeof(TModel),
            OptionsName = optionsName
        });
        return this;
    }
}
