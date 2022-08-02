// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Request;

public class ChangeEnabledStatusRequest : BaseSchedulerJobRequest
{
    public bool Enabled { get; set; }
}
