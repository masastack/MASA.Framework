// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.DistributedLock.Local;

public class DefaultLocalDistributedLock : IDistributedLock
{
    private readonly MemoryCache<string, SemaphoreSlim> _localObjects = new();

    public IDisposable? TryGet(string key, TimeSpan timeout = default)
    {
        var semaphore = GetSemaphoreSlim(key);

        if (!semaphore.Wait(timeout))
        {
            return null;
        }

        return new DisposeAction(semaphore);
    }

    public async Task<IAsyncDisposable?> TryGetAsync(string key, TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        var semaphore = GetSemaphoreSlim(key);

        if (!await semaphore.WaitAsync(timeout, cancellationToken))
        {
            return null;
        }

        return new DisposeAction(semaphore);
    }

    private SemaphoreSlim GetSemaphoreSlim(string key)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return _localObjects.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
    }
}
