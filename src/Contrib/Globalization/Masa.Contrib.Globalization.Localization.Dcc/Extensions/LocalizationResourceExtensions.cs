// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Globalization.Localization;

public static class LocalizationResourceExtensions
{
    public static LocalizationResource UseDcc(
        this LocalizationResource localizationResource,
        string appId,
        string configObject,
        params LanguageInfo[] languages)
    {
        var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetRequiredService<IMasaConfiguration>();
        foreach (var language in languages)
        {
            var resourceContributor = new DccLocalizationResourceContributor(appId, configObject, language.Culture, masaConfiguration);
            localizationResource.AddContributor(language.Culture, resourceContributor);
        }
        return localizationResource;
    }
}
