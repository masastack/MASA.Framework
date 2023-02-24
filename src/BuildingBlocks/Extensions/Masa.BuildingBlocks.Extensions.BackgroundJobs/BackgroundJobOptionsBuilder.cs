﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class BackgroundJobOptionsBuilder
{
    public IServiceCollection Services { get; }

    public BackgroundJobOptionsBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
