// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Domain.Repositories;

public interface IOrderRepository : IRepository<Orders>
{
    Task AddAsync(Orders order);
}

public interface ICustomizeOrderRepository : IRepository<Orders, int>
{

}
