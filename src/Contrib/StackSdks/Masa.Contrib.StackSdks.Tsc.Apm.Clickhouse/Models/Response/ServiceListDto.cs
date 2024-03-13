// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Response;

public class ServiceListDto
{
    public string Service { get; set; }

    public string Name { get; set; }

    public IEnumerable<string> Envs { get; set; }

    public long Latency { get; set; }

    public double Throughput { get; set; }

    public double Failed { get; set; }
}

public class ChartLineDto
{
    public string Name { get; set; }

    public IEnumerable<ChartLineItemDto> Currents { get; set; }

    public IEnumerable<ChartLineItemDto> Previous { get; set; }
}

public class ChartLineCountDto
{
    public string Name { get; set; }

    public IEnumerable<ChartLineCountItemDto> Currents { get; set; }

    public IEnumerable<ChartLineCountItemDto> Previous { get; set; }
}

public class ChartLineCountItemDto
{
    public long Time { get; set; }

    public object Value { get; set; }
}

public class ChartLineItemDto
{
    public long Time { get; set; }

    public long Latency { get; set; }

    public double P99 { get; set; }

    public double P95 { get; set; }

    public double Throughput { get; set; }

    public double Failed { get; set; }
}

public class ChartPointDto
{
    public string X { get; set; }

    public string Y { get; set; }
}
