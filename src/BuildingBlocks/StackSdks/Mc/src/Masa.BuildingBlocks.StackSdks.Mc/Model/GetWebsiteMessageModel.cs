// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class GetWebsiteMessageModel : PaginatedOptionsModel
{
    public string Filter { get; set; } = string.Empty;

    public WebsiteMessageFilterType? FilterType { get; set; }

    public Guid? ChannelId { get; set; }

    public bool? IsRead { get; set; }
}
