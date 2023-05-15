// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.UoW;

public interface IUnitOfWorkManager
{
    /// <summary>
    /// Create new DbContext
    /// We create DbContext with lazy loading enabled by default
    /// </summary>
    /// <param name="lazyLoading">Deferred creation of DbContext, easy to specify tenant or environment by yourself, which is very effective for physical isolation</param>
    /// <returns></returns>
    IUnitOfWork CreateDbContext(bool lazyLoading = true);

    IUnitOfWork CreateDbContext(DbContextConnectionStringOptions connectionStringOptions);
}
