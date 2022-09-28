// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    public static void UseMySQL(this MedallionBuilder medallionBuilder,
        string connectionString,
        Action<MySqlConnectionOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new MySqlDistributedSynchronizationProvider(connectionString, options));
    }

    public static void UseMySQL<TDbContextType>(this MedallionBuilder medallionBuilder,
        Action<MySqlConnectionOptionsBuilder>? options = null)
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
                return new MySqlDistributedSynchronizationProvider(connectionString, options);
            }
        });
    }
}
