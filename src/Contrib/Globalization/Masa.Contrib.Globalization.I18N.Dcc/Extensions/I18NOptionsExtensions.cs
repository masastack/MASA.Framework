// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18N;

public static class I18NOptionsExtensions
{
    public static void UseDcc(this I18NOptions i18NOptions)
        => i18NOptions.UseDcc(Dcc.Internal.Constant.CULTURES_NAME_PREFIX);

    public static void UseDcc(
        this I18NOptions i18NOptions,
        string configObjectPrefix)
        => i18NOptions.UseDcc(DccConfig.AppId, configObjectPrefix);

    public static void UseDcc(
        this I18NOptions i18NOptions,
        string appId,
        string configObjectPrefix)
    {
        I18NResourceResourceConfiguration
            .Resources
            .Add<DefaultResource>()
            .UseDcc(appId, configObjectPrefix, i18NOptions.SupportedCultures);
    }
}
