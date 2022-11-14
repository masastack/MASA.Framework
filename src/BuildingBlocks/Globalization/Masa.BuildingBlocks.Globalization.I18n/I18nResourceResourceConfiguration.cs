// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

public static class I18nResourceResourceConfiguration
{
    public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    public static I18nResourceDictionary Resources { get; set; } = new();
}
