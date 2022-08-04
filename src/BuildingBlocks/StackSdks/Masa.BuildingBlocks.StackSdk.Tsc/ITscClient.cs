// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc;

public interface ITscClient
{
    public ILogService LogService { get; }

    public IMetricService MetricService { get; }
}
