// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Model;

public class LoggerBody
{
    public LoggerTypes LoggerType { get; set; }

    public Guid TaskId { get; set; }

    public Guid JobId { get; set; }

    public WriterTypes Writer { get; set; }
}
