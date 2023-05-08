// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter.Tests;

public class CustomDaprProcess : DaprProcessBase
{
    public CustomDaprProcess(
        IDaprEnvironmentProvider daprEnvironmentProvider,
        IDaprProvider daprProvider,
        IOptionsMonitor<DaprOptions> daprOptions)
        : base(daprEnvironmentProvider, daprProvider, daprOptions)
    {
    }
}
