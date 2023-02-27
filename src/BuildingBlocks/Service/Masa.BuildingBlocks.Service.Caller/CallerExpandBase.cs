// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public abstract class CallerExpandBase : CallerBase
{
    private ICaller? _caller;
    public ILogger<CallerExpandBase>? Logger => ServiceProvider!.GetService<ILogger<CallerExpandBase>>();

    protected CallerExpandBase() : base() { }

    protected CallerExpandBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    protected override ICaller GetCaller()
    {
        if (_caller == null)
        {
            _caller = ServiceProvider!.GetRequiredService<ICallerFactory>().Create(Name!);
            if (_caller is ICallerExpand callerExpand)
            {
                callerExpand.ConfigRequestMessage(ConfigHttpRequestMessageAsync);
            }
            else
            {
                Logger?.LogDebug("----- caller does not implement ICallerExpand, callerName: {Name}", Name);
            }
        }
        return _caller;
    }
}
