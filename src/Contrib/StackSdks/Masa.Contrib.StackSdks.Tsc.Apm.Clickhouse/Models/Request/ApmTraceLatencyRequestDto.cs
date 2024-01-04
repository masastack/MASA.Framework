// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Request;

public class ApmTraceLatencyRequestDto : ApmEndpointRequestDto
{
    public long? LatMin { get; set; }

    public long? LatMax { get; set; }

    public new int PageSize { get; } = 1;
}
