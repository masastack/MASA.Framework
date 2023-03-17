// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Model;

public class MetricMonitorItemModel
{
    public bool IsExpression { get; set; }

    public string Expression { get; set; } = string.Empty;

    public MetricAggregationModel Aggregation { get; set; } = new();

    public string Alias { get; set; } = string.Empty;

    public bool IsOffset { get; set; }

    public int OffsetPeriod { get; set; }
}
