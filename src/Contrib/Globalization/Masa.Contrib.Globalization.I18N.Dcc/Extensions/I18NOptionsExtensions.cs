// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18N;

public static class I18NOptionsExtensions
{
    public static I18NOptions UseDcc(
        this I18NOptions localization,
        params LanguageInfo[] languages)
        => localization.UseDcc(DccConfig.AppId, "I18n", languages);

    public static I18NOptions UseDcc(
        this I18NOptions localization,
        string configObject,
        params LanguageInfo[] languages)
        => localization.UseDcc(DccConfig.AppId, configObject, languages);

    public static I18NOptions UseDcc(
        this I18NOptions localization,
        string appId,
        string configObject,
        params LanguageInfo[] languages)
    {
        var services = MasaApp.GetServices();
        services.Configure<MasaI18NOptions>(options =>
        {
            if (languages.Length == 0)
            {
                var serviceProvider = services.BuildServiceProvider();
                var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
                languages = configurationApiClient.GetAsync<List<LanguageInfo>>(
                    $"{configObject}.{Dcc.Internal.Const.SUPPORTED_CULTURES_NAME}",
                    newLanguages
                        =>
                    {
                        options.Languages = newLanguages.ToList();
                    }).ConfigureAwait(false).GetAwaiter().GetResult().ToArray();
            }
            options.Languages = languages.ToList();
            options.Resources
                .Add<DefaultResource>()
                .UseDcc(appId, configObject, languages);
        });
        return localization;
    }
}
