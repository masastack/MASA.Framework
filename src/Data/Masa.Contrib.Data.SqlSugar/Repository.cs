// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using SqlSugar;

namespace Masa.Contrib.Data.SqlSugar
{
    public class Repository<TEntity> : SimpleClient<TEntity>, IRepository<TEntity> where TEntity : class, new()
    {
        public Repository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
