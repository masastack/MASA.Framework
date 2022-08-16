// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;

public interface IAuditEntity<out TUserId> : IEntity
{
    TUserId Creator { get; }

    DateTime CreationTime { get; }

    TUserId Modifier { get; }

    DateTime ModificationTime { get; }
}

public interface IAuditEntity<out TKey, out TUserId> : IAuditEntity<TUserId>, IEntity<TKey>
{
}
