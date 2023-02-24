// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

public class BackgroundJobInfo
{
    public Guid Id { get; set; }

    public string JobArgs { get; set; }

    public short Times { get; set; } = 0;

    public short MaxRetryTimes { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime NextTryTime { get; set; }

    public bool IsInvalid { get; set; } = false;
}
