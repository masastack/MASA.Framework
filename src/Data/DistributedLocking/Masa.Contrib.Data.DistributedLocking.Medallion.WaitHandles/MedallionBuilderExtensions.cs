// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    public static void UseWaitHandles(this MedallionBuilder medallionBuilder,
        TimeSpan? abandonmentCheckCadence = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new WaitHandleDistributedSynchronizationProvider(abandonmentCheckCadence));
    }
}
