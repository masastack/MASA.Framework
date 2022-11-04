// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18N;

public static class I18NOptionsExtensions
{
    public static void UseDcc(this I18NOptions i18NOptions)
        => i18NOptions.UseDcc(Dcc.Internal.Constant.DEFAULT_CONFIG_OBJECT_NAME, Dcc.Internal.Constant.SUPPORTED_CULTURES_NAME);

    public static void UseDcc(
        this I18NOptions i18NOptions,
        string configObject)
        => i18NOptions.UseDcc(DccConfig.AppId, configObject);

    public static void UseDcc(
        this I18NOptions i18NOptions,
        string appId,
        string configObject)
    {
        I18NResourceResourceConfiguration
            .Resources
            .Add<DefaultResource>()
            .UseDcc(appId, configObject, i18NOptions.SupportedCultures);
    }
}
