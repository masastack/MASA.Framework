// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class GetChannelModel : PaginatedOptions
{
    public string Filter { get; set; } = string.Empty;

    public ChannelTypes? Type { get; set; }

    public string DisplayName { get; set; } = string.Empty;
}
