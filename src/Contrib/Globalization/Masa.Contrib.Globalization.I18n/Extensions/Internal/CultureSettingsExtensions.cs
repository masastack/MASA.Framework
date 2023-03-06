// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Globalization.I18n.Tests")]
namespace Masa.Contrib.Globalization.I18n.Extensions.Internal;

internal static class CultureSettingsExtensions
{
#pragma warning disable CS0618
    public static string GetSupportCultureFileName(this CultureSettings cultureSettings)
    {
        var cultureFileName = cultureSettings.SupportCultureFileName.IsNullOrWhiteSpace() ? cultureSettings.SupportCultureName :
            cultureSettings.SupportCultureFileName;
        return cultureFileName.IsNullOrWhiteSpace() ? ContribI18nConstant.SUPPORTED_CULTURES_NAME : cultureFileName;
    }
#pragma warning restore CS0618
}
