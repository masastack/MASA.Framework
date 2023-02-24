// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class JobContext
{
    public IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// Job Class Type
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Job parameters
    /// </summary>
    public object Args { get; set; }
}
