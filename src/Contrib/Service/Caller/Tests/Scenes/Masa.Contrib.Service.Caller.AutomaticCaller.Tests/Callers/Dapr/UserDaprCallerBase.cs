// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers;

public abstract class UserDaprCallerBase : DaprCallerBase
{
    protected override string AppId { get; set; } = "User-Service";

    protected UserDaprCallerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public string GetAppId() => AppId;

    protected override MasaDaprClientBuilder UseDapr()
    {
        return base.UseDapr().AddHttpRequestMessage<DefaultDaprRequestMessage>();
    }
}
