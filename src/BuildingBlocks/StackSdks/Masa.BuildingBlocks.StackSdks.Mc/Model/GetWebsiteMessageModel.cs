// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class GetWebsiteMessageModel : PaginatedOptions
{
    public string Filter { get; set; } = string.Empty;

    public WebsiteMessageFilterType? FilterType { get; set; }

    public Guid? ChannelId { get; set; }

    public string ChannelCode { get; set; } = string.Empty;

    public bool? IsRead { get; set; }

    public string Tag { get; set; } = string.Empty;

    public GetWebsiteMessageModel(int page, int pageSize, Dictionary<string, bool>? sorting = null) : base(page, pageSize, sorting)
    {

    }
}
