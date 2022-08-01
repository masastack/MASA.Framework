﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DependencyInjection.Repository.Tests;

public interface IRepository<TEntity> : IScopedDependency
    where TEntity : class
{

}

public interface IRepository<TDbContext, TEntity> : IScopedDependency
    where TEntity : class
    where TDbContext : class
{

}
