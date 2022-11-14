// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18n;

public static class I18nResourceExtensions
{
    public static I18nResource UseDcc(
        this I18nResource i18nResource,
        string appId,
        string configObjectPrefix,
        List<CultureModel> supportedCultures)
    {
        var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetRequiredService<IMasaConfiguration>();
        var contributors = supportedCultures
            .Select(supportedCulture => new DccI18nResourceContributor(appId, configObjectPrefix, supportedCulture.Culture, masaConfiguration)).ToList();
        foreach (var contributor in contributors)
        {
            i18nResource.AddContributor(contributor.CultureName, contributor);
        }
        return i18nResource;
    }
}
