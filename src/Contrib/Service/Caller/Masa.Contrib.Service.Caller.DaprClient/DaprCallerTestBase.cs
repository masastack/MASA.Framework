// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public abstract class DaprCallerTestBase : DaprCallerBase
{
    private readonly Dapr.Client.DaprClient _daprClient;

    protected DaprCallerTestBase(Dapr.Client.DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    protected override MasaDaprClientBuilder UseDapr()
        => CallerOptions.UseDaprTest(Name!, AppId, _daprClient);
}
