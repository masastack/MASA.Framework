// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization;

public class JsonFileLocalizationResourceContributor : FileLocalizationResourceContributorBase
{
    private readonly JsonSerializerOptions _deserializeOptions;

    public JsonFileLocalizationResourceContributor(
        Type resourceType,
        string cultureName,
        string filePath,
        JsonSerializerOptions deserializeOptions,
        ILoggerFactory? loggerFactory) : base(resourceType, cultureName, filePath, loggerFactory)
    {
        _deserializeOptions = deserializeOptions;
    }

    protected override Dictionary<string, LocalizedString> ParseResourceFromFileContent(string fileContent)
    {
        JsonLocalizationFile jsonFile;
        try
        {
            jsonFile = JsonSerializer.Deserialize<JsonLocalizationFile>(fileContent, _deserializeOptions) ??
                throw new UserFriendlyException($"Can not parse json string. json file path: {fileContent}");
        }
        catch (JsonException ex)
        {
            throw new UserFriendlyException("Can not parse json string. " + ex.Message);
        }
        var cultureCode = jsonFile.Culture;
        if (string.IsNullOrEmpty(cultureCode))
        {
            throw new UserFriendlyException("Culture is empty in language json file.");
        }

        var dictionary = new Dictionary<string, LocalizedString>();
        var duplicateNames = new List<string>();
        foreach (var item in jsonFile.Texts)
        {
            if (string.IsNullOrEmpty(item.Key))
            {
                throw new UserFriendlyException("The key is empty in given json string.");
            }

            if (dictionary.ContainsKey(item.Key))
            {
                duplicateNames.Add(item.Key);
            }

            dictionary[item.Key] = new LocalizedString(item.Key,
                item.Value.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine));
        }

        if (duplicateNames.Count > 0)
        {
            throw new UserFriendlyException(
                "A dictionary can not contain same key twice. There are some duplicated names: " +
                string.Join(", ", duplicateNames));
        }

        return dictionary;
    }
}
