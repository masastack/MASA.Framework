// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n;

public abstract class FileConfigurationProvider : ConfigurationProvider
{
    private readonly string _resourceTypeName;
    private readonly string _languageDirectory;
    private readonly IEnumerable<string> _cultureNames;
    private readonly bool _useMasaConfiguration;
    private readonly Dictionary<string, Dictionary<string, string>> _dictionary;

    protected FileConfigurationProvider(MasaJsonConfigurationSource configurationSource)
    {
        _resourceTypeName = configurationSource.ResourceType.Name;
        _languageDirectory = configurationSource.LanguageDirectory;
        _cultureNames = configurationSource.CultureNames;
        _useMasaConfiguration = configurationSource.UseMasaConfiguration;
        _dictionary = new(StringComparer.OrdinalIgnoreCase);
    }

    public override void Load()
    {
        foreach (var cultureName in _cultureNames)
        {
            var configuration = InitializeConfiguration(_languageDirectory, cultureName);
            _dictionary[FormatKey(cultureName)] = configuration.ConvertToDictionary();
        }

        Data = FormatData();
    }

    private string FormatKey(string cultureName)
    {
        string localSection = BuildingBlocks.Globalization.I18n.Constant.DEFAULT_LOCAL_SECTION;
        var list = _useMasaConfiguration ?
            new List<string>
            {
                SectionTypes.Local.ToString(),
                localSection,
                _resourceTypeName,
                cultureName
            } :
            new List<string>
            {
                localSection,
                _resourceTypeName,
                cultureName
            };
        return string.Join(ConfigurationPath.KeyDelimiter, list);
    }

    private IConfiguration InitializeConfiguration(string basePath, string cultureName)
        => AddFile(new ConfigurationBuilder(), basePath, cultureName).Build();

    private Dictionary<string, string> FormatData()
    {
        var data = new Dictionary<string, string>();
        foreach (var item in _dictionary)
        {
            foreach (var resource in item.Value)
            {
                data[$"{item.Key}{ConfigurationPath.KeyDelimiter}{resource.Key}"] = resource.Value;
            }
        }
        return data;
    }

    protected abstract IConfigurationBuilder AddFile(IConfigurationBuilder configurationBuilder, string basePath, string cultureName);

    public void Initialize()
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        foreach (var cultureName in _cultureNames)
        {
            configurationBuilder = AddFile(configurationBuilder, _languageDirectory, cultureName);
        }
        var configuration = configurationBuilder.Build();
        ChangeToken.OnChange(() => configuration.GetReloadToken(), Load);
    }
}
