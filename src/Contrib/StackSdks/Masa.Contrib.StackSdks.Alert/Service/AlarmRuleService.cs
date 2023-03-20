// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Alert.Service;

public class AlarmRuleService : IAlarmRuleService
{
    readonly ICaller _caller;
    readonly string _party = "api/v1/AlarmRules";

    public AlarmRuleService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<Guid> CreateAsync(AlarmRuleUpsertModel options)
    {
        var requestUri = $"{_party}";
        return await _caller.PostAsync<AlarmRuleUpsertModel, Guid>(requestUri, options);
    }

    public async Task UpdateAsync(Guid id, AlarmRuleUpsertModel options)
    {
        var requestUri = $"{_party}/{id}";
        await _caller.PutAsync(requestUri, options);
    }

    public async Task DeleteAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        await _caller.DeleteAsync(requestUri, null);
    }

    public async Task<AlarmRuleModel?> GetAsync(Guid id)
    {
        var requestUri = $"{_party}/{id}";
        return await _caller.GetAsync<AlarmRuleModel>(requestUri);
    }
}
