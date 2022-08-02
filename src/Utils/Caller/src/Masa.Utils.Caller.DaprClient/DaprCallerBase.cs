﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.DaprClient;

public abstract class DaprCallerBase : CallerBase
{
    protected abstract string AppId { get; set; }

    public virtual Action<DaprClientBuilder>? Configure { get; set; } = null;

    protected DaprCallerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override void UseCallerExtension() => UseDapr();

    protected virtual void UseDapr()
    {
        CallerOptions.UseDapr(opt =>
        {
            opt.Name = Name;
            opt.AppId = AppId;
            if (Configure != null)
            {
                opt.Configure = Configure;
            }
        });
    }
}
