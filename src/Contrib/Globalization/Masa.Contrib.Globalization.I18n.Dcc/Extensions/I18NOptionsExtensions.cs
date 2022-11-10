// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18n;

public static class I18NOptionsExtensions
{
    [ExcludeFromCodeCoverage]
    public static void UseDcc(this I18NOptions i18NOptions)
        => i18NOptions.UseDcc(Dcc.Internal.Constant.CULTURES_NAME_PREFIX);

    [ExcludeFromCodeCoverage]
    public static void UseDcc(
        this I18NOptions i18NOptions,
        string configObjectPrefix)
        => i18NOptions.UseDcc(DccConfig.AppId, configObjectPrefix);

    public static void UseDcc(
        this I18NOptions i18NOptions,
        string appId,
        string configObjectPrefix)
    {
        i18NOptions.Services.Configure<MasaI18NOptions>(options =>
        {
            options.Resources.TryAdd<DccResource>(resource =>
            {
                resource.UseDcc(appId, configObjectPrefix, i18NOptions.SupportedCultures);
            });
            options.Resources.GetOrNull<DefaultResource>()?.TryAddBaseResourceTypes<DccResource>();
        });
    }
}
