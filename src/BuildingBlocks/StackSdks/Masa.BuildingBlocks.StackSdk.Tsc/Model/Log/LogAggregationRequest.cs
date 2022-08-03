// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Model;

public class LogAggregationRequest
{
    public IEnumerable<FieldAggregationRequest> FieldMaps { get; set; }

    public string Query { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}
