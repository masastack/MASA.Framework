// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Request;

public class AddSchedulerJobRequest
{
    public string ProjectIdentity { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsAlertException { get; set; }

    public JobTypes JobType { get; set; }

    public Guid OperatorId { get; set; }

    public string CronExpression { get; set; } = string.Empty;

    public string JobIdentity { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Default Ignore
    /// </summary>
    public ScheduleExpiredStrategyTypes ScheduleExpiredStrategy { get; set; } = ScheduleExpiredStrategyTypes.Ignore;

    /// <summary>
    /// Default Parallel
    /// </summary>
    public ScheduleBlockStrategyTypes ScheduleBlockStrategy { get; set; } = ScheduleBlockStrategyTypes.Serial;

    /// <summary>
    /// Default IgnoreTimeout
    /// </summary>
    public RunTimeoutStrategyTypes RunTimeoutStrategy { get; set; } = RunTimeoutStrategyTypes.IgnoreTimeout;

    public int RunTimeoutSecond { get; set; }

    public int FailedRetryInterval { get; set; }

    public int FailedRetryCount { get; set; }

    public SchedulerJobAppConfig? JobAppConfig { get; set; }

    public SchedulerJobHttpConfig? HttpConfig { get; set; }

    public SchedulerJobDaprServiceInvocationConfig? DaprServiceInvocationConfig { get; set; }
}
