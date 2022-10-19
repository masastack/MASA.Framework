// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18N;

public static class I18NResourceExtensions
{
    public static I18NResource UseDcc(
        this I18NResource localizationResource,
        string appId,
        string configObject,
        params LanguageInfo[] languages)
    {
        var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetRequiredService<IMasaConfiguration>();
        var contributors = languages.Select(language => new DccI18NResourceContributor(appId, configObject, language.Culture, masaConfiguration)).ToList();
        foreach (var contributor in contributors)
        {
            localizationResource.AddContributor(contributor.CultureName, contributor);
        }
        return localizationResource;
    }
}
