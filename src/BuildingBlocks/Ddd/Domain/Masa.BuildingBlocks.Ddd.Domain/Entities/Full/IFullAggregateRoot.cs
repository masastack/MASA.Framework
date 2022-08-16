// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities.Full;

public interface IFullAggregateRoot<out TUserId> : IFullEntity<TUserId>, IAuditAggregateRoot<TUserId>
{

}

public interface IFullAggregateRoot<TKey, out TUserId> : IFullEntity<TKey, TUserId>, IAuditAggregateRoot<TKey, TUserId>
{

}
