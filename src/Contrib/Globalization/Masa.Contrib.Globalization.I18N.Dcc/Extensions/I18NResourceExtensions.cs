// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18N;

public static class I18NResourceExtensions
{
    public static I18NResource UseDcc(
        this I18NResource i18NResource,
        string appId,
        string configObjectPrefix,
        List<CultureModel> supportedCultures)
    {
        var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetRequiredService<IMasaConfiguration>();
        var contributors = supportedCultures
            .Select(supportedCulture => new DccI18NResourceContributor(appId, configObjectPrefix, supportedCulture.Culture, masaConfiguration)).ToList();
        foreach (var contributor in contributors)
        {
            i18NResource.AddContributor(contributor.CultureName, contributor);
        }
        return i18NResource;
    }
}
