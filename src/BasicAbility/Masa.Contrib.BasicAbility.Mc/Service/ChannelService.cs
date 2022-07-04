// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Service;

public class ChannelService : IChannelService
{
    readonly ICallerProvider _callerProvider;
    readonly string _party = "api/channel";

    public ChannelService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<ChannelModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _callerProvider.GetAsync<ChannelModel>(requestUri);
    }

    public async Task<PaginatedListModel<ChannelModel>> GetListAsync(GetChannelModel options)
    {
        var requestUri = $"{_party}";
        return await _callerProvider.GetAsync<GetChannelModel, PaginatedListModel<ChannelModel>>(requestUri, options) ?? new();
    }
}
