// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    /// <summary>
    /// used to coordinate between processes on the same machine
    /// </summary>
    /// <param name="medallionBuilder"></param>
    /// <param name="abandonmentCheckCadence">specifies how frequently the implementation will check to see if the original holder of a lock/semaphore abandoned it without properly releasing it while waiting for it to become available. Defaults to 2s</param>
    public static void UseWaitHandles(this MedallionBuilder medallionBuilder,
        TimeSpan? abandonmentCheckCadence = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new WaitHandleDistributedSynchronizationProvider(abandonmentCheckCadence));
    }
}
