// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    public static void UseOracle(this MedallionBuilder medallionBuilder,
        string connectionString,
        Action<OracleConnectionOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new OracleDistributedSynchronizationProvider(connectionString, options));
    }

    // /// <summary>
    // /// todo: only use a fixed database link string, waiting for Isolation to support temporary changes to multi-environment and multi-tenancy
    // /// </summary>
    // /// <param name="medallionBuilder"></param>
    // /// <param name="options"></param>
    // /// <typeparam name="TDbContextType"></typeparam>
    // public static void UseOracle<TDbContextType>(this MedallionBuilder medallionBuilder,
    //     Action<OracleConnectionOptionsBuilder>? options = null)
    // {
    //     medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(serviceProvider
    //         =>
    //     {
    //         var name = ConnectionStringNameAttribute.GetConnStringName(typeof(TDbContextType));
    //         var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
    //         var connectionString = connectionStringProvider.GetConnectionString(name);
    //         return new OracleDistributedSynchronizationProvider(connectionString, options);
    //     });
    // }
}
