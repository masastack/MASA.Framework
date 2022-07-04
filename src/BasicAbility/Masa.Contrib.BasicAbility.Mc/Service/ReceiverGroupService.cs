// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Service;

public class ReceiverGroupService : IReceiverGroupService
{
    readonly ICallerProvider _callerProvider;
    readonly string _party = "api/receiver-group";

    public ReceiverGroupService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<ReceiverGroupModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _callerProvider.GetAsync<ReceiverGroupModel>(requestUri);
    }

    public async Task<PaginatedListModel<ReceiverGroupModel>> GetListAsync(GetReceiverGroupModel options)
    {
        var requestUri = $"{_party}";
        return await _callerProvider.GetAsync<GetReceiverGroupModel, PaginatedListModel<ReceiverGroupModel>>(requestUri, options) ?? new();
    }
}
