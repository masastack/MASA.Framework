// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Model;

public class SchedulerJobAppConfig
{
    public string JobAppIdentity { get; set; } = string.Empty;

    public string JobEntryAssembly { get; set; } = string.Empty;

    public string JobEntryClassName { get; set; } = string.Empty;

    public string JobParams { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;
}
