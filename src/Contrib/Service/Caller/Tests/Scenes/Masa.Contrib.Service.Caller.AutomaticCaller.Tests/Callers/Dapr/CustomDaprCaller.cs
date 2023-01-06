// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers;

public class CustomDaprCaller : DaprCallerTestBase
{
    protected override string AppId { get; set; } = nameof(CustomDaprCaller);

    public CustomDaprCaller(Dapr.Client.DaprClient daprClient) : base(daprClient)
    {
    }

    public Task<string> TestGetString() => Caller.GetStringAsync("masastack");

    protected override DefaultDaprClientBuilder UseDapr()
    {
        return base.UseDapr().AddHttpRequestMessage<DefaultDaprRequestMessage>();
    }
}
