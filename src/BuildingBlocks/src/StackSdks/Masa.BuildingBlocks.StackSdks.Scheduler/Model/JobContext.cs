// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Model;

public class JobContext
{
    public Guid JobId { get; set; }

    public Guid TaskId { get; set; }

    public DateTimeOffset ExecutionTime { get; set; }

    public string ExcuteClassName { get; set; } = string.Empty;

    public List<string> ExcuteParameters { get; set; } = new();

    public object? ExcuteResult { get; set; }
}
