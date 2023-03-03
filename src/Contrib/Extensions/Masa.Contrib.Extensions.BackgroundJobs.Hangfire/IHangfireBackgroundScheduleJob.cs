// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Hangfire;

public interface IHangfireBackgroundScheduleJob : IBackgroundScheduleJob
{
    TimeZoneInfo TimeZone  { get; set; }

    string Queue  { get; set; }
}
