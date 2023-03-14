// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.BuildingBlocks.Data")]
// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class ScopedService
{
    public IServiceProvider ServiceProvider { get; set; }

    public ScopedService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
