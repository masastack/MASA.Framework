// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class ServiceSingleton
{
    public IServiceProvider ServiceProvider { get; set; }

    public ServiceSingleton(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
