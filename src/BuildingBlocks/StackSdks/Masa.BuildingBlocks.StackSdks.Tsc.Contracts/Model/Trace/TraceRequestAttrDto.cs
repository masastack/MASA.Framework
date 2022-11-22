// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Trace;

public class TraceRequestAttrDto
{
    public string Service { get; set; }

    public string Instance { get; set; }

    public string Endpoint { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Query { get; set; }

    public int MaxCount { get; set; }
}
