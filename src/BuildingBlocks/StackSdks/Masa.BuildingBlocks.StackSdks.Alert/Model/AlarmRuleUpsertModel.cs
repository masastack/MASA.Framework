// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Model;

public class AlarmRuleUpsertModel
{
    public string DisplayName { get; set; } = string.Empty;

    public AlarmRuleTypes Type { get; set; }

    public string ProjectIdentity { get; set; } = string.Empty;

    public string AppIdentity { get; set; } = string.Empty;

    public string ChartYAxisUnit { get; set; } = string.Empty;

    public CheckFrequencyModel CheckFrequency { get; set; } = new();

    public string WhereExpression { get; set; } = string.Empty;

    public int ContinuousTriggerThreshold { get; set; }

    public SilenceCycleModel SilenceCycle { get; set; } = new();

    public bool IsEnabled { get; set; }

    public List<LogMonitorItemModel> LogMonitorItems { get; set; } = new();

    public List<MetricMonitorItemModel> MetricMonitorItems { get; set; } = new();

    public List<AlarmRuleItemModel> Items { get; set; } = new();
}
