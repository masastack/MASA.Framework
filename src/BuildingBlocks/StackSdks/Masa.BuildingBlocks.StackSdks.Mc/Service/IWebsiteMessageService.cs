// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Service;

public interface IWebsiteMessageService
{
    Task<PaginatedListModel<WebsiteMessageModel>> GetListAsync(GetWebsiteMessageModel options);

    Task<WebsiteMessageModel?> GetAsync(Guid id);

    Task<List<WebsiteMessageChannelModel>> GetChannelListAsync();

    Task SetAllReadAsync(ReadAllWebsiteMessageModel options);

    Task DeleteAsync(Guid id);

    Task ReadAsync(ReadWebsiteMessageModel options);

    Task CheckAsync();

    Task<List<WebsiteMessageModel>> GetNoticeListAsync(GetNoticeListModel options);

    Task SendCheckNotificationAsync();

    Task SendGetNotificationAsync(List<string> userIds);

    Task<List<WebsiteMessageTagModel>> GetListByTagAsync(List<string> tags, string channelCode);

    Task<int> GetUnreadAsync(GetUnreadModel options);
}
