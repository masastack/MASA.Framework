// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class CallerRelationOptions : MasaRelationOptions<ICallerDisposeWrapper>
{
    public ServiceLifetime? Lifetime { get; set; }

    public CallerRelationOptions(string name, Func<IServiceProvider, ICallerDisposeWrapper> func, ServiceLifetime? lifetime)
        : base(name, func)
    {
        Lifetime = lifetime;
    }
}
