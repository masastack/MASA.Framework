// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class BackgroundJobContext
{
    public IServiceProvider ServiceProvider { get; }

    public BackgroundJobContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
