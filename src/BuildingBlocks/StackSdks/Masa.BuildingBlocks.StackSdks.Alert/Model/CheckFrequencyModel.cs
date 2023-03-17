// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Model;

public class CheckFrequencyModel
{
    public AlarmCheckFrequencyTypes Type { get; set; }

    public TimeIntervalModel FixedInterval { get; set; } = new();

    public string CronExpression { get; set; } = string.Empty;
}
