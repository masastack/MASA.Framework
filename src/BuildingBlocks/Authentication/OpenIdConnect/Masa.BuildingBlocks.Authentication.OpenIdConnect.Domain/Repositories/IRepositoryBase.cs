// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Repositories;

public interface IRepositoryBase<TEntity> where TEntity : class, IEntity
{
    Task<PaginatedList<TEntity>> GetPaginatedListAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? condition = null);

    Task<TEntity?> GetDetailAsync(Guid id);

    Task<List<TEntity>> GetListAsync();

    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);

    Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate);

    ValueTask<TEntity> AddAsync(TEntity entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task RemoveAsync(TEntity entity);
}

