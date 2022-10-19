﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Globalization.I18N;

public class MasaI18NOptions
{
    public I18NResourceDictionary Resources { get; }

    public MasaI18NOptions()
    {
        Resources = new();
    }
}
