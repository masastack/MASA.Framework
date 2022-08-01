// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class AuditEntityModel<TKey, TUserId> : EntityModel<TKey>
{
    public TUserId Creator { get; set; } = default!;

    public DateTime CreationTime { get; set; }

    public TUserId Modifier { get; set; } = default!;

    public DateTime ModificationTime { get; set; }
}
