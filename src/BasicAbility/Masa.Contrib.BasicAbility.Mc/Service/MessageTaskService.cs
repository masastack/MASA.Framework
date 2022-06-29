// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Service;

public class MessageTaskService : IMessageTaskService
{
    readonly ICallerProvider _callerProvider;
    readonly string _party = "api/message-task";

    public MessageTaskService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<MessageTaskModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _callerProvider.GetAsync<MessageTaskModel>(requestUri);
    }

    public async Task SendOrdinaryMessageAsync(SendOrdinaryMessageModel options)
    {
        var requestUri = $"{_party}";
        await _callerProvider.PostAsync(requestUri, (MessageTaskUpsertModel)options);
    }

    public async Task SendTemplateMessageAsync(SendTemplateMessageModel options)
    {
        var requestUri = $"{_party}";
        await _callerProvider.PostAsync(requestUri, (MessageTaskUpsertModel)options);
    }
}
