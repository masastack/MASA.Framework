// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class BackgroundJobOptionsBuilder
{
    public IServiceCollection Services { get; }

    public bool DisableBackgroundJob { get; set; } = false;

    /// <summary>
    /// The collection of assemblies where the background Job task resides
    /// Use global assembly when null
    /// </summary>
    public IEnumerable<Assembly>? Assemblies { get; set; }

    public BackgroundJobOptionsBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
