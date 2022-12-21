// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests.Infrastructure;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{

}

public class Repository<TDbContext, TEntity> : Repository<TEntity>
    where TEntity : class
{

}
