// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Service;

public class ReceiverGroupService : IReceiverGroupService
{
    readonly ICaller _caller;
    readonly string _party = "api/receiver-group";

    public ReceiverGroupService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<ReceiverGroupModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _caller.GetAsync<ReceiverGroupModel>(requestUri);
    }

    public async Task<PaginatedListModel<ReceiverGroupModel>> GetListAsync(GetReceiverGroupModel options)
    {
        var requestUri = $"{_party}";
        return await _caller.GetAsync<GetReceiverGroupModel, PaginatedListModel<ReceiverGroupModel>>(requestUri, options) ?? new();
    }
}
