// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18N;

public static class I18NOptionsExtensions
{
    public static void UseDcc(
        this I18NOptions i18NOptions,
        params LanguageInfo[] languages)
        => i18NOptions.UseDcc(Dcc.Internal.Constant.DEFAULT_CONFIG_OBJECT_NAME, Dcc.Internal.Constant.SUPPORTED_CULTURES_NAME, languages);

    public static void UseDcc(
        this I18NOptions i18NOptions,
        string configObject,
        string supportCultureName,
        params LanguageInfo[] languages)
        => i18NOptions.UseDcc(DccConfig.AppId, configObject, supportCultureName, languages);

    public static void UseDcc(
        this I18NOptions i18NOptions,
        string appId,
        string configObject,
        string supportCultureName,
        params LanguageInfo[] languages)
    {
        var services = MasaApp.GetServices();
        if (languages.Length == 0)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
            languages = configurationApiClient.GetAsync<List<LanguageInfo>>(
                $"{configObject}.{supportCultureName}",
                newLanguages
                    =>
                {
                    I18NResourceResourceConfiguration.Languages = newLanguages;
                    I18NResourceResourceConfiguration
                        .Resources
                        .Add<DefaultResource>()
                        .UseDcc(appId, configObject, newLanguages.ToArray());
                }).ConfigureAwait(false).GetAwaiter().GetResult().ToArray();
        }
        I18NResourceResourceConfiguration.Languages = languages.ToList();
        I18NResourceResourceConfiguration
            .Resources
            .Add<DefaultResource>()
            .UseDcc(appId, configObject, languages);
    }
}
