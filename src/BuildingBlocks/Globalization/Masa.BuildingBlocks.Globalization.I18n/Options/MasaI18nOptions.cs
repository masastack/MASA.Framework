// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18n;

public class MasaI18nOptions
{
    public I18nResourceDictionary Resources { get; }

    public MasaI18nOptions()
    {
        Resources = new();
    }
}
