// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Model;

public class FieldAggregationRequest
{
    public string Name { get; set; }

    public string Alias { get; set; }

    public AggregationTypes AggregationType { get; set; }
}
