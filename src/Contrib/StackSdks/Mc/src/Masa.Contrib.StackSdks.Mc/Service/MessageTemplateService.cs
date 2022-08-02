// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Mc.Service;

public class MessageTemplateService : IMessageTemplateService
{
    readonly ICaller _caller;
    readonly string _party = "api/message-template";

    public MessageTemplateService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<MessageTemplateModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _caller.GetAsync<MessageTemplateModel>(requestUri);
    }

    public async Task<PaginatedListModel<MessageTemplateModel>> GetListAsync(GetMessageTemplateModel options)
    {
        var requestUri = $"{_party}";
        return await _caller.GetAsync<GetMessageTemplateModel, PaginatedListModel<MessageTemplateModel>>(requestUri, options) ?? new();
    }
}
