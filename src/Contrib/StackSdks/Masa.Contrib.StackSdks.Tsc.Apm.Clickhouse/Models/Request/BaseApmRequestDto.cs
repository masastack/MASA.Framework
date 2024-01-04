// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Request;

public class BaseApmRequestDto : RequestPageBase
{
    public string? Env { get; set; }

    public ComparisonTypes? ComparisonType { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Period { get; set; }

    public string? Service { get; set; }

    public string? Queries { get; set; }

    public string? OrderField { get; set; }

    public bool? IsDesc { get; set; }

    public string StatusCodes { get; set; }

    internal int[] ErrorStatusCodes => string.IsNullOrEmpty(StatusCodes) ? Constants.DefaultErrorStatus : StatusCodes.Split(',').Select(s => Convert.ToInt32(s)).Where(num => num != 0).ToArray();

    internal bool? IsServer { get; set; }
}
