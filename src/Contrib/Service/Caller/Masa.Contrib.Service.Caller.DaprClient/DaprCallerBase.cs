// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public abstract class DaprCallerBase : CallerBase
{
    protected abstract string AppId { get; set; }

    protected virtual Action<DaprClientBuilder>? Configure { get; set; } = null;

    protected DaprCallerBase()
    {
    }

    protected DaprCallerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override void UseCallerExtension() => UseDapr();

    protected virtual void UseDaprPre()
    {
    }

    MasaDaprClientBuilder UseDapr()
    {
        UseDaprPre();

        var daprClientBuilder = CallerOptions.UseDapr(callerClient =>
        {
            callerClient.AppId = AppId;
            ConfigMasaCallerClient(callerClient);
        }, Configure);

        UseDaprPost(daprClientBuilder);
        return daprClientBuilder;
    }

    protected virtual void UseDaprPost(MasaDaprClientBuilder masaHttpClientBuilder)
    {

    }
}
