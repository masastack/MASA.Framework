// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18n;

public static class I18nOptionsExtensions
{
    [ExcludeFromCodeCoverage]
    public static void UseDcc(this I18nOptions i18nOptions)
        => i18nOptions.UseDcc(DccConstant.CULTURES_NAME_PREFIX);

    [ExcludeFromCodeCoverage]
    public static void UseDcc(
        this I18nOptions i18nOptions,
        string configObjectPrefix)
        => i18nOptions.UseDcc(DccConfig.AppId, configObjectPrefix);

    public static void UseDcc(
        this I18nOptions i18nOptions,
        string appId,
        string configObjectPrefix)
    {
        i18nOptions.Services.Configure<MasaI18nOptions>(options =>
        {
            options.Resources.TryAdd<DccResource>(resource =>
            {
                resource.UseDcc(appId, configObjectPrefix, i18nOptions.SupportedCultures);
            });
            options.Resources.GetOrNull<DefaultResource>()?.TryAddBaseResourceTypes<DccResource>();
        });
    }
}
