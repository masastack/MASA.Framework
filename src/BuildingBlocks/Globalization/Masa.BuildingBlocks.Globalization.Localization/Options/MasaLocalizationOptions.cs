// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Localization;

public class MasaLocalizationOptions
{
    public Type? DefaultResourceType { get; set; }

    public LocalizationResourceDictionary Resources { get; }

    public MasaLocalizationOptions()
    {
        Resources = new ();
    }
}
