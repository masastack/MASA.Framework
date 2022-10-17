// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization;

public abstract class FileLocalizationConfigurationProvider : ConfigurationProvider
{
    protected IConfigurationBuilder ConfigurationBuilder;
    private readonly string _resourceType;
    private readonly List<string> _filePaths;
    private readonly bool _useMasaConfiguration;
    private readonly Dictionary<string, Dictionary<string, string>> _dictionary;
    private readonly IConfiguration _configuration;

    public FileLocalizationConfigurationProvider(JsonLocalizationConfigurationSource configurationSource)
    {
        ConfigurationBuilder = new ConfigurationBuilder();
        _resourceType = configurationSource.ResourceType.Name;
        _filePaths = configurationSource.LocalizationFilePaths;
        _useMasaConfiguration = configurationSource.UseMasaConfiguration;
        _dictionary = new();

        _filePaths.ForEach(filePath =>
        {
            var file = new FileInfo(filePath);
            AddFile(file.Directory!.FullName, file.Name);
        });
        _configuration = ConfigurationBuilder.Build();
        ChangeToken.OnChange(() => _configuration.GetReloadToken(), Load);
    }

    public override void Load()
    {
        _filePaths.ForEach(filePath =>
        {
            var file = new FileInfo(filePath);
            var configuration = InitializeConfiguration(file.Directory!.FullName, file.Name);
            var culture = configuration.GetValue<string>(Const.CULTURE);
            LocalizationResourceConfiguration.Dictionary[filePath] = culture;
            var dictionary = configuration.ConvertToDictionary();
            dictionary.Remove(Const.CULTURE);
            _dictionary[FormatKey(culture)] = dictionary.ToDictionary(
                keyValuePair => keyValuePair.Key.TrimStart($"{Const.TESTS}:"),
                keyValuePair => keyValuePair.Value);
        });

        Data = FormatData();
    }

    private string FormatKey(string cultureName)
        => _useMasaConfiguration ?
            string.Join(ConfigurationPath.KeyDelimiter, new List<string>()
            {
                SectionTypes.Local.ToString(),
                Const.DEFAULT_LOCAL_SECTION,
                _resourceType,
                cultureName
            })
            : string.Join(ConfigurationPath.KeyDelimiter, new List<string>()
            {
                Const.DEFAULT_LOCAL_SECTION,
                _resourceType,
                cultureName
            });

    private IConfiguration InitializeConfiguration(string basePath, string fileName)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(fileName, false, true)
            .Build();
        return configuration;
    }

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

    protected abstract void AddFile(string basePath, string fileName);
}
