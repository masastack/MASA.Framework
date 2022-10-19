// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using Microsoft.Extensions.FileProviders;

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
        var services = MasaApp.GetServices();

        languageDirectory = PathHelper.GetAndCheckLanguageDirectory(languageDirectory);

        if (languages.Length == 0)
        {
            languages = GetLanguageInfos(languageDirectory, supportCultureName).ToArray();
            MonitorChange(languageDirectory, supportCultureName, () =>
            {
                var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
                var options = serviceProvider.GetRequiredService<IOptions<MasaI18NOptions>>();
                options.Value.Languages = GetLanguageInfos(languageDirectory, supportCultureName).ToList();
            });
        }

        services.Configure<MasaI18NOptions>(options =>
        {
            options.Languages = languages.ToList();

            options.Resources
                .Add<DefaultResource>()
                .AddJson(languageDirectory, languages);
        });
    }

    private static List<LanguageInfo> GetLanguageInfos(
        string languageDirectory,
        string supportCultureName)
    {
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
