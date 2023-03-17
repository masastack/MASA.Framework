// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Model;

public class SilenceCycleModel
{
    public SilenceCycleTypes Type { get; set; }

    public TimeIntervalModel TimeInterval { get; set; } = new();

    public int SilenceCycleValue { get; set; }
}
