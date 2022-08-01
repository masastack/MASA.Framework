// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Model;

public class ChannelModel : AuditEntityModel<Guid, Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public ChannelTypes Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;
}
