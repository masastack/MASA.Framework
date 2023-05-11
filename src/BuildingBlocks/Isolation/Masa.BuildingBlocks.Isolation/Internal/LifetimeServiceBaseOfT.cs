// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

internal class LifetimeServiceBase<TService> : LifetimeServiceBase, IDisposable
{
    public TService Service { get; }

    public LifetimeServiceBase(TService service)
    {
        Service = service;
    }

    public override object GetService() => Service!;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Service is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
