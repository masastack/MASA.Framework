// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using SqlSugar;

namespace Masa.Contrib.Data.SqlSugar
{
    public interface IRepository<TEntity> : ISimpleClient<TEntity>, IScopedDependency where TEntity : class, new()
    {
    }
}
