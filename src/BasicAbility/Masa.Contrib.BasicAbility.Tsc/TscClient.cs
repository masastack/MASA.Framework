// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.BasicAbility.Tsc;

namespace Masa.Contrib.BasicAbility.Tsc;

internal class TscClient : ITscClient
{
    public ILogService LogService { get; private set; }
    public IMetricService MetricService { get; private set; }
}
