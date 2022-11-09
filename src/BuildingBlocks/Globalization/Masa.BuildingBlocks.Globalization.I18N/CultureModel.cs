// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class CultureModel
{
    public string Culture { get; set; }

    public string? DisplayName { get; set; }

    public string Icon { get; set; }

    public CultureModel(string culture, string? displayName = null, string? icon = null)
    {
        Culture = culture;
        DisplayName = displayName;
        Icon = icon ?? string.Empty;
    }
}
