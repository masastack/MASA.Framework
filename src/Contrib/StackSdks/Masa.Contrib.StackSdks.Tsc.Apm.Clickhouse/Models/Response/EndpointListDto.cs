// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Response;

public class EndpointListDto
{
    public string Name { get; set; }

    public string Method { get; set; }

    public string Service { get; set; }

    public string AgentType { get; set; }

    public long Latency { get; set; }

    public double Failed { get; set; }

    public double Throughput { get; set; }
}
