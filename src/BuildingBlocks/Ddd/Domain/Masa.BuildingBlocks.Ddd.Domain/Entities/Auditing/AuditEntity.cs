// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;

public abstract class AuditEntity<TUserId> : Entity, IAuditEntity<TUserId>
{
    public TUserId Creator { get; protected set; } = default!;

    public DateTime CreationTime { get; protected set; }

    public TUserId Modifier { get; protected set; } = default!;

    public DateTime ModificationTime { get; set; }

    protected AuditEntity() => Initialize();

    private void Initialize()
    {
        this.CreationTime = this.GetCurrentTime();
        this.ModificationTime = this.GetCurrentTime();
    }

    protected virtual DateTime GetCurrentTime() => DateTime.UtcNow;
}

public abstract class AuditEntity<TKey, TUserId> : Entity<TKey>, IAuditEntity<TKey, TUserId>
{
    public TUserId Creator { get; protected set; } = default!;

    public DateTime CreationTime { get; protected set; }

    public TUserId Modifier { get; protected set; } = default!;

    public DateTime ModificationTime { get; protected set; }

    protected AuditEntity() : base()
    {
        Initialize();
    }

    protected AuditEntity(TKey id) : base(id)
    {
        Initialize();
    }

    private void Initialize()
    {
        this.CreationTime = this.GetCurrentTime();
        this.ModificationTime = this.GetCurrentTime();
    }

    protected virtual DateTime GetCurrentTime() => DateTime.UtcNow;
}
