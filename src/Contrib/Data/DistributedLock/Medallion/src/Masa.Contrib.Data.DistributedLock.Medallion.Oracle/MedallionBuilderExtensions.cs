﻿// Copyright (c) MASA Stack All rights reserved.
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

    public static void UseOracle<TDbContextType>(this MedallionBuilder medallionBuilder,
        Action<OracleConnectionOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(serviceProvider
            =>
        {
            var unitOfWorkManager = serviceProvider.GetService<IUnitOfWorkManager>();
            if (unitOfWorkManager == null)
                throw new NotSupportedException("UoW is not supported");

            using (var unitOfWork = unitOfWorkManager.CreateDbContext())
            {
                var name = ConnectionStringNameAttribute.GetConnStringName(typeof(TDbContextType));
                var connectionStringProvider = unitOfWork.ServiceProvider.GetRequiredService<IConnectionStringProvider>();
                var connectionString = connectionStringProvider.GetConnectionString(name);
                return new OracleDistributedSynchronizationProvider(connectionString, options);
            }
        });
    }
}
