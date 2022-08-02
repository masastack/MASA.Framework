// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Tsc.Service;

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Tests")]
namespace Masa.Contrib.StackSdks.Tsc;

internal class TscClient : ITscClient
{
    public TscClient(ICaller caller)
    {
        LogService = new LogService(caller);
        MetricService = new MetricService(caller);
    }

    public ILogService LogService { get; }

    public IMetricService MetricService { get; }
}
