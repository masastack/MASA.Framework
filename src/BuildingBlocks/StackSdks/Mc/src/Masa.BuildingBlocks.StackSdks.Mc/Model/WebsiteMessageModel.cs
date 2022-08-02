// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class WebsiteMessageModel : AuditEntityModel<Guid, Guid>
{
    public Guid ChannelId { get; set; }

    public ChannelModel Channel { get; set; } = default!;

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string LinkUrl { get; set; } = string.Empty;

    public DateTimeOffset SendTime { get; set; }

    public bool IsRead { get; set; }

    public DateTimeOffset? ReadTime { get; set; }

    public string Abstract { get; set; } = string.Empty;

    public Guid PrevId { get; set; }

    public Guid NextId { get; set; }
}
