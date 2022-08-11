// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities.Full;

public interface IFullEntity<out TUserId> : IAuditEntity<TUserId>, ISoftDelete
{

}

public interface IFullEntity<TKey, out TUserId> : IAuditEntity<TKey, TUserId>, ISoftDelete
{

}
