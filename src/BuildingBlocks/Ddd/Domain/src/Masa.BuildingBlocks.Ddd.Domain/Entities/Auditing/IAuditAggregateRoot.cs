// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;

public interface IAuditAggregateRoot<TUserId> : IAuditEntity<TUserId>, IAggregateRoot
{

}

public interface IAuditAggregateRoot<TKey, TUserId> : IAuditEntity<TUserId>, IAggregateRoot<TKey>
{

}
