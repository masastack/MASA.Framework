// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Service;

public class MessageTaskService : IMessageTaskService
{
    readonly ICaller _caller;
    readonly string _party = "api/message-task";

    public MessageTaskService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<MessageTaskModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _caller.GetAsync<MessageTaskModel>(requestUri);
    }

    public async Task SendOrdinaryMessageAsync(SendOrdinaryMessageModel options)
    {
        var requestUri = $"{_party}/SendOrdinaryMessage";
        await _caller.PostAsync(requestUri, options);
    }

    public async Task SendTemplateMessageAsync(SendTemplateMessageModel options)
    {
        var requestUri = $"{_party}/SendTemplateMessage";
        await _caller.PostAsync(requestUri, options);
    }
}
