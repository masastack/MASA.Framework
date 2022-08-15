// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Model
{
    public class SchedulerJobModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public bool IsAlertException { get; set; }

        public string CronExpression { get; set; } = string.Empty;

        public string JobIdentity { get; set; } = string.Empty;

        public JobTypes JobType { get; set; }

        public string SpecifiedWorkerHost { get; set; } = string.Empty;

        public ScheduleExpiredStrategyTypes ScheduleExpiredStrategy { get; set; }

        public ScheduleBlockStrategyTypes ScheduleBlockStrategy { get; set; }

        public RunTimeoutStrategyTypes RunTimeoutStrategy { get; set; }

        public int RunTimeoutSecond { get; set; }

        public int FailedRetryInterval { get; set; }

        public int FailedRetryCount { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool Enabled { get; set; }

        public Guid BelongTeamId { get; set; }

        public string BelongProjectIdentity { get; set; } = string.Empty;

        public string Origin { get; set; } = string.Empty;

        public int SortPriority { get; set; }

        public Guid Creator { get; set; }

        public DateTimeOffset UpdateExpiredStrategyTime { get; set; }

        public DateTimeOffset LastScheduleTime { get; set; } = DateTimeOffset.MinValue;

        public DateTimeOffset LastRunStartTime { get; set; } = DateTimeOffset.MinValue;

        public DateTimeOffset LastRunEndTime { get; set; } = DateTimeOffset.MinValue;

        public SchedulerJobAppConfig JobAppConfig { get; set; } = new();

        public SchedulerJobHttpConfig HttpConfig { get; set; } = new();

        public SchedulerJobDaprServiceInvocationConfig DaprServiceInvocationConfig { get; set; } = new();

        public DateTime CreationTime { get; set; }

        public DateTime ModificationTime { get; set; }
    }
}
