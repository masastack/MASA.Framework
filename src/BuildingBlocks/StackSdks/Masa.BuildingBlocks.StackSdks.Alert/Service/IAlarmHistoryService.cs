// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Service;

public interface IAlarmHistoryService
{
    Task ChangeHandlerAsync(Guid id, Guid handler);

    Task CompletedAsync(Guid id);
}
