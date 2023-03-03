﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public abstract class BackgroundScheduleJobBase : IBackgroundScheduleJob
{
    public string Id { get; set; }

    public string CronExpression { get; set; }

    public abstract Task ExecuteAsync(IServiceProvider serviceProvider);
}
