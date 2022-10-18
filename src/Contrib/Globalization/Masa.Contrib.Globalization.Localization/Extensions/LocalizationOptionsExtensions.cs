// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization.Extensions;

public static class MasaLocalizationOptionsExtensions
{
    public static void UseJson(this LocalizationOptions _,
        string languageDirectory,
        params string[] cultureNames)
    {
        var services = MasaApp.GetServices();
        services.Configure<MasaLocalizationOptions>(options =>
        {
            options.Resources
                .Add<DefaultResource>()
                .AddJson(languageDirectory, cultureNames);
        });
    }
}
