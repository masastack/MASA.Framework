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
        params string[] cultureNames)
    {
        var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
        var masaConfiguration = serviceProvider.GetRequiredService<IMasaConfiguration>();
        foreach (var cultureName in cultureNames)
        {
            var resourceContributor = new DccLocalizationResourceContributor(appId, configObject, cultureName, masaConfiguration);
            localizationResource.AddContributor(cultureName, resourceContributor);
        }
        return localizationResource;
    }
}
