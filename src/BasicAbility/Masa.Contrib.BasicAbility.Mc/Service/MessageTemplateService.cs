// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Service;

public class MessageTemplateService : IMessageTemplateService
{
    readonly ICallerProvider _callerProvider;
    readonly string _party = "api/message-template";

    public MessageTemplateService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<MessageTemplateModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _callerProvider.GetAsync<MessageTemplateModel>(requestUri);
    }

    public async Task<PaginatedListModel<MessageTemplateModel>> GetListAsync(GetMessageTemplateModel options)
    {
        var requestUri = $"{_party}";
        return await _callerProvider.GetAsync<GetMessageTemplateModel, PaginatedListModel<MessageTemplateModel>>(requestUri, options) ?? new();
    }
}
