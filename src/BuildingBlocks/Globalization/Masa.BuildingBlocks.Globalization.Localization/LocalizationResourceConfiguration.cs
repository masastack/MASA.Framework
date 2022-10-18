// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.Localization;

public static class LocalizationResourceConfiguration
{
    public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

    public static string DefaultCultureName { get; set; }
}
