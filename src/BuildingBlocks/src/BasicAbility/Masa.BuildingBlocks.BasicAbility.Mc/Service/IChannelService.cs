// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Service;

public interface IChannelService
{
    Task<ChannelModel?> GetAsync(Guid id);

    Task<PaginatedListModel<ChannelModel>> GetListAsync(GetChannelModel options);
}
