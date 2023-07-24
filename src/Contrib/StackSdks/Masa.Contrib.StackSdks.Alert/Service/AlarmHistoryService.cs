// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Alert.Service;

public class AlarmHistoryService : IAlarmHistoryService
{
    readonly ICaller _caller;
    readonly string _party = "api/v1/AlarmHistories";

    public AlarmHistoryService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task ChangeHandlerAsync(Guid id, Guid handler)
    {
        var requestUri = $"{_party}/{id}/change-handler?handler={handler}";
        await _caller.PostAsync(requestUri, new { });
    }

    public async Task CompletedAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}/complete";
        await _caller.PostAsync(requestUri, new { });
    }
}
