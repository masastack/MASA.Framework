// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Repositories;

public class PaginatedList<TEntity> : PaginatedListBase<TEntity>
    where TEntity : class, IEntity
{
}
