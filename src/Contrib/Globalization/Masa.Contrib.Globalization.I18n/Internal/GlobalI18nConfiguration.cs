// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Internal;

internal static class GlobalI18nConfiguration
{
    private static List<CultureModel> _supportedCultures;

    public static void SetSupportedCultures(List<CultureModel> supportedCultures) => _supportedCultures = supportedCultures;

    public static List<CultureModel> GetSupportedCultures() => _supportedCultures;
}
