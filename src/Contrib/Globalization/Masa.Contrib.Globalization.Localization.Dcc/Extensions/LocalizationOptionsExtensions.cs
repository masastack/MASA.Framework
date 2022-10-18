// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Globalization.Localization;

public static class LocalizationOptionsExtensions
{
    public static LocalizationOptions UseDcc(
        this LocalizationOptions localization,
        params string[] cultureNames)
        => localization.UseDcc(DccConfig.AppId, "I18n", cultureNames);

    public static LocalizationOptions UseDcc(
        this LocalizationOptions localization,
        string configObject,
        params string[] cultureNames)
        => localization.UseDcc(DccConfig.AppId, configObject, cultureNames);

    public static LocalizationOptions UseDcc(
        this LocalizationOptions localization,
        string appId,
        string configObject,
        params string[] cultureNames)
    {
        var services = MasaApp.GetServices();
        services.Configure<MasaLocalizationOptions>(options =>
        {
            options.Resources
                .Add<DefaultResource>()
                .UseDcc(appId, configObject, cultureNames);
        });
        return localization;
    }
}
