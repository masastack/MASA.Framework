// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.BasicAbility.Tsc.Tests")]
namespace Masa.Contrib.BasicAbility.Tsc;

internal class TscClient : ITscClient
{
    public TscClient(ICallerProvider callerProvider)
    {
        LogService = new LogService(callerProvider);
        MetricService = new MetricService(callerProvider);
    }

    public ILogService LogService { get; }

    public IMetricService MetricService { get; }
}
