// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Scheduler.Model;

public class SchedulerJobDaprServiceInvocationConfig
{
    public string MethodName { get; set; } = string.Empty;

    public HttpMethods HttpMethod { get; set; }

    public string Data { get; set; } = string.Empty;

    public string DaprServiceIdentity { get; set; } = string.Empty;
}
