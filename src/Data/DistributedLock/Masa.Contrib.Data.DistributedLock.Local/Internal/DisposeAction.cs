// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.DistributedLock.Local.Internal;

internal class DisposeAction : IDisposable, IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public DisposeAction(SemaphoreSlim semaphore) => _semaphore = semaphore;

    public ValueTask DisposeAsync()
    {
        _semaphore.Release();
        return ValueTask.CompletedTask;
    }

    public void Dispose() => _semaphore.Release();
}
