// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IMasaDatabaseApi
{
    EntityState EntityState { get; set; }

    CommitState CommitState { get; set; }

    ValueTask<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
