// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities.Full;

public abstract class FullEntity<TUserId>
    : AuditEntity<TUserId>, IFullEntity<TUserId>
{
    public bool IsDeleted { get; protected set; }
}

public abstract class FullEntity<TKey, TUserId>
    : AuditEntity<TKey, TUserId>, IFullEntity<TKey, TUserId>
{
    public bool IsDeleted { get; protected set; }

    public FullEntity() : base()
    {
    }

    public FullEntity(TKey id) : base(id)
    {
    }
}
