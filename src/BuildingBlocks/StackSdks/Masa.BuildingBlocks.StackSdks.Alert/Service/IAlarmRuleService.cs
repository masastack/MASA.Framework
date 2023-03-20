// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Service;

public interface IAlarmRuleService
{
    Task<Guid> CreateAsync(AlarmRuleUpsertModel options);

    Task UpdateAsync(Guid id, AlarmRuleUpsertModel options);

    Task DeleteAsync(Guid id);

    Task<AlarmRuleModel?> GetAsync(Guid id);
}
