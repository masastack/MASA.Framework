// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class WebsiteMessageTagModel
{
    public string Tag { get; set; } = string.Empty;

    public int Unread { get; set; }

    public Guid ChannelId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTimeOffset SendTime { get; set; }

    public Guid UserId { get; set; }
}
