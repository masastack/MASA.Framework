// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Response;

public class EndpointChartDto
{
    public IEnumerable<ChartPointDto> P99s { get; set; }

    public IEnumerable<ChartPointDto> P95s { get; set; }

    public IEnumerable<ChartPointDto> Latencies { get; set; }

    public IEnumerable<ChartPointDto> Throughputs { get; set; }

    public IEnumerable<ChartPointDto> Fails { get; set; }
}

public class ErrorMessageDto
{
    public string Type { get; set; }

    public string Message { get; set; }

    public DateTime LastTime { get; set; }

    public int Total { get; set; }
}
