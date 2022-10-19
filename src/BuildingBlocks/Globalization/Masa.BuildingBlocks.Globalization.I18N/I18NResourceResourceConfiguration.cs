// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public static class I18NResourceResourceConfiguration
{
    public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    public static I18NResourceDictionary Resources { get; set; } = new();

    public static IEnumerable<LanguageInfo> Languages { get; set; } = new List<LanguageInfo>();
}
