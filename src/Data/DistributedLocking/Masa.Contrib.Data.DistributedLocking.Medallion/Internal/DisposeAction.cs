// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.DistributedLocking.Medallion.Internal;

internal class DisposeAction : IDisposable, IAsyncDisposable
{
    private readonly IDistributedSynchronizationHandle _handle;

    public DisposeAction(IDistributedSynchronizationHandle handle) => _handle = handle;

    public ValueTask DisposeAsync()
    {
        _handle.DisposeAsync();
        return ValueTask.CompletedTask;
    }

    public void Dispose() => _handle.Dispose();
}
