// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Model;

public class MetricAggregationModel
{
    public string Name { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;

    public MetricComparisonOperator ComparisonOperator { get; set; } = MetricComparisonOperator.Equal;

    public string Value { get; set; } = string.Empty;

    public MetricAggregationType AggregationType { get; set; } = MetricAggregationType.Count;
}
