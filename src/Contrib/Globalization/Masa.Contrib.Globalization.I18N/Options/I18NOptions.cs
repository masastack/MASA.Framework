// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.I18N;

public class I18NOptions
{
    public IServiceCollection Services { get; }

    public I18NOptions(IServiceCollection services)
    {
        Services = services;
    }
}
