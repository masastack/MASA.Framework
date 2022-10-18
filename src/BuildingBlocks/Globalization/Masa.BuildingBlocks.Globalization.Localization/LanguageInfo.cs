﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.Localization;

public class LanguageInfo
{
    public string Culture { get; set; }

    public string Display { get; set; }

    public string Icon { get; set; }

    public LanguageInfo(string culture, string? display = null)
    {
        Culture = culture;
        Display = display ?? culture;
    }
}
