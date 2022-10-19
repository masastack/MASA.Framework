// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using Masa.BuildingBlocks.Globalization.I18N.Options;

namespace Masa.Contrib.Globalization.I18N;

public static class I18NOptionsExtensions
{
    public static void UseJson(this I18NOptions _,
        params LanguageInfo[] languages)
        => _.UseJson(Internal.Const.DEFAULT_RESOURCE_PATH, Internal.Const.SUPPORTED_CULTURES_NAME, languages);

    public static void UseJson(this I18NOptions _,
        string languageDirectory,
        params LanguageInfo[] languages)
        => _.UseJson(languageDirectory, Internal.Const.SUPPORTED_CULTURES_NAME, languages);

    public static void UseJson(this I18NOptions _,
        string languageDirectory,
        string supportCultureName,
        params LanguageInfo[] languages)
    {
        languageDirectory = PathHelper.GetAndCheckLanguageDirectory(languageDirectory);

        if (languages.Length == 0)
        {
            languages = GetLanguageInfos(languageDirectory, supportCultureName).ToArray();
            MonitorChange(languageDirectory, supportCultureName, () =>
            {
                I18NResourceResourceConfiguration.Languages = GetLanguageInfos(languageDirectory, supportCultureName).ToList();
                I18NResourceResourceConfiguration
                    .Resources
                    .Add<DefaultResource>()
                    .AddJson(languageDirectory, I18NResourceResourceConfiguration.Languages.ToArray());
            });
        }

        I18NResourceResourceConfiguration.Languages = languages.ToList();
        I18NResourceResourceConfiguration
            .Resources
            .Add<DefaultResource>()
            .AddJson(languageDirectory, languages);
    }

    private static List<LanguageInfo> GetLanguageInfos(
        string languageDirectory,
        string supportCultureName)
    {
        start:
        try
        {
            using var fileStream = new FileStream(Path.Combine(languageDirectory, supportCultureName),FileMode.Open);
            if (!fileStream.CanRead)
            {
                Task.Delay(300);
                goto start;
            }
        }
        catch (IOException)
        {
            Task.Delay(300);
            goto start;
        }
        var supportCultureFilePath = Path.Combine(languageDirectory, supportCultureName);
        var content = File.ReadAllText(supportCultureFilePath);
        return System.Text.Json.JsonSerializer.Deserialize<List<LanguageInfo>>(content)!;
    }

    private static void MonitorChange(string filePath, string supportCultureName, Action onChange)
    {
        var fileProvider = new PhysicalFileProvider(filePath);
        ChangeToken.OnChange(
            () => fileProvider.Watch(supportCultureName),
            onChange
        );
    }
}
