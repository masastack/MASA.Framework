// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Trace;

public class TraceRequestDto : BaseRequestDto
{
    public string TraceId { get; set; }

    public string Service { get; set; }

    public string Instance { get; set; }

    public string Endpoint { get; set; }
}
