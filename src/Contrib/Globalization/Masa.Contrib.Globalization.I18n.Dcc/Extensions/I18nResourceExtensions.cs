// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18n;

// ReSharper disable once InconsistentNaming
public static class I18nResourceExtensions
{
    // ReSharper disable once InconsistentNaming
    public static I18nResource UseDcc(
        this I18nResource i18nResource,
        string appId,
        string configObjectPrefix,
        params CultureModel[] supportedCultures)
        => i18nResource.UseDcc(appId, configObjectPrefix, supportedCultures.ToList());

    // ReSharper disable once InconsistentNaming
    public static I18nResource UseDcc(
        this I18nResource i18nResource,
        string appId,
        string configObjectPrefix,
        List<CultureModel> supportedCultures)
    {
        var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetRequiredService<IMasaConfiguration>();
        var contributors =
            (supportedCultures.Any() ? supportedCultures : serviceProvider.GetRequiredService<IOptions<CultureSettings>>().Value.SupportedCultures)
            .Select(supportedCulture => new DccI18nResourceContributor(appId, configObjectPrefix, supportedCulture.Culture, masaConfiguration))
            .ToList();
        foreach (var contributor in contributors)
        {
            i18nResource.AddContributor(contributor.CultureName, contributor);
        }
        return i18nResource;
    }
}
