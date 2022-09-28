// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    public static void UseZooKeeper(this MedallionBuilder medallionBuilder,
        string connectionString,
        Action<ZooKeeperDistributedSynchronizationOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new ZooKeeperDistributedSynchronizationProvider(connectionString, options));
    }

    public static void UseZooKeeper(this MedallionBuilder medallionBuilder,
        ZooKeeperPath directoryPath,
        string connectionString,
        Action<ZooKeeperDistributedSynchronizationOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new ZooKeeperDistributedSynchronizationProvider(directoryPath, connectionString, options));
    }
}
