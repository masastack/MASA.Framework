// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18n;

public class I18nOptions
{
    public IServiceCollection Services { get; }

    public List<CultureModel> SupportedCultures { get; }

    public I18nOptions(IServiceCollection services, List<CultureModel> supportedCultures)
    {
        Services = services;
        SupportedCultures = supportedCultures;
    }
}
