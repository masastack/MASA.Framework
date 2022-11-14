// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

public class LanguageInfo
{
    public string Culture { get; set; }

    public string DisplayName { get; set; }

    public string Icon { get; set; }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Content displayed for the current language
    /// </summary>
    public string? UIDisplayName { get; set; }

    public LanguageInfo(string culture, string displayName, string? icon = null)
    {
        Culture = culture;
        DisplayName = displayName;
        Icon = icon ?? string.Empty;
    }
}
