// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public abstract class DaprCallerBase : CallerBase
{
    protected abstract string AppId { get; set; }

    public virtual Action<DaprClientBuilder>? Configure { get; set; } = null;

    protected DaprCallerBase()
    {
    }

    protected DaprCallerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override void UseCallerExtension() => UseDapr();

    protected virtual MasaDaprClientBuilder UseDapr()
    {
        var daprClientBuilder = CallerOptions.UseDapr(callerClient =>
        {
            callerClient.AppId = AppId;
            ConfigMasaCallerClient(callerClient);
        }, Configure);
        return daprClientBuilder;
    }

    protected virtual void ConfigMasaCallerClient(MasaCallerClient callerClient)
    {
    }
}
