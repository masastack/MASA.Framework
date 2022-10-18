// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization.Extensions;

public static class MasaLocalizationOptionsExtensions
{
    public static void UseJson(this LocalizationOptions _,
        string languageDirectory,
        params LanguageInfo[] languages)
    {
        var services = MasaApp.GetServices();
        if (languages.Length == 0)
            languages = GetLanguageInfos(languageDirectory).ToArray();

        services.Configure<MasaLocalizationOptions>(options =>
        {
            options.Languages = languages.ToList();

            options.Resources
                .Add<DefaultResource>()
                .AddJson(languageDirectory, languages);
        });
    }

    private static List<LanguageInfo> GetLanguageInfos(string languageDirectory)
    {
        var supportCultureFilePath = Path.Combine(languageDirectory, Internal.Const.SUPPORTED_CULTURES_FILE_NAME);
        var content = File.ReadAllText(supportCultureFilePath);
        return System.Text.Json.JsonSerializer.Deserialize<List<LanguageInfo>>(content) ?? new List<LanguageInfo>()
        {
            new("en-US")
        };
    }
}
