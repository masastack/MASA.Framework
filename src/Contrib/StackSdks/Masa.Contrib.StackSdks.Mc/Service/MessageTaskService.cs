// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Mc.Service;

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

    public async Task SendOrdinaryMessageByInternalAsync(SendOrdinaryMessageByInternalModel options)
    {
        var requestUri = $"{_party}/SendOrdinaryMessageByInternal";
        await _caller.PostAsync(requestUri, options);
    }

    public async Task SendTemplateMessageByInternalAsync(SendTemplateMessageByInternalModel options)
    {
        var requestUri = $"{_party}/SendTemplateMessageByInternal";
        await _caller.PostAsync(requestUri, options);
    }

    public async Task SendOrdinaryMessageByExternalAsync(SendOrdinaryMessageByExternalModel options)
    {
        var requestUri = $"{_party}/SendOrdinaryMessageByExternal";
        await _caller.PostAsync(requestUri, options);
    }

    public async Task SendTemplateMessageByExternalAsync(SendTemplateMessageByExternalModel options)
    {
        var requestUri = $"{_party}/SendTemplateMessageByExternal";
        await _caller.PostAsync(requestUri, options);
    }

    public async Task<PaginatedListModel<MessageTaskModel>> GetListAsync(GetMessageTaskModel options)
    {
        var requestUri = $"{_party}";
        return await _caller.GetAsync<GetMessageTaskModel, PaginatedListModel<MessageTaskModel>>(requestUri, options) ?? new();
    }

    public async Task DeleteAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        await _caller.DeleteAsync(requestUri, null);
    }

    public async Task UpdateAsync(Guid id, MessageTaskUpsertModel messageTask)
    {
        var requestUri = $"{_party}/{id}";
        await _caller.PutAsync(requestUri, messageTask);
    }

    public async Task SetIsEnabledAsync(Guid id, bool isEnabled)
    {
        var requestUri = $"{_party}/{id}/enabled/{isEnabled}";
        await _caller.PutAsync(requestUri, new { });
    }

    public async Task WithdrawnAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}/Withdrawn";
        await _caller.PostAsync(requestUri, new { });
    }

    public async Task ResendAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}/Resend";
        await _caller.PostAsync(requestUri, new { });
    }
}
