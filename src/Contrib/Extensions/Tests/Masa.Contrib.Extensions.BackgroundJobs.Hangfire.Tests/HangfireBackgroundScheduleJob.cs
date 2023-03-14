// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Memory;

public class HangfireBackgroundScheduleJob: IHangfireBackgroundScheduleJob
{
    public string Id { get; set; }
    public string CronExpression { get; set; }

    public TimeZoneInfo TimeZone { get; set; }
    public string Queue { get; set; }

    public Task ExecuteAsync(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }
}
