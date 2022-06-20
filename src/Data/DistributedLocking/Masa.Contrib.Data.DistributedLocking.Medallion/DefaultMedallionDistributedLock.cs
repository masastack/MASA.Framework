// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.DistributedLocking.Medallion;

public class DefaultMedallionDistributedLock : IMasaDistributedLock
{
    private readonly IDistributedLockProvider _distributedLockProvider;

    public DefaultMedallionDistributedLock(IDistributedLockProvider distributedLockProvider)
        => _distributedLockProvider = distributedLockProvider;

    public IDisposable? TryGet(string key, TimeSpan timeout)
    {
        ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(key);
        var handle = _distributedLockProvider.TryAcquireLock(key, timeout);
        if (handle == null)
        {
            return null;
        }
        return new DisposeAction(handle);
    }

    public async Task<IAsyncDisposable?> TryGetAsync(string key, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(key);
        var handle = await _distributedLockProvider.TryAcquireLockAsync(key, timeout, cancellationToken);
        if (handle == null)
        {
            return null;
        }

        return new DisposeAction(handle);
    }
}
