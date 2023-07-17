// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Alert;

public class AlertClient : IAlertClient
{
    public AlertClient(ICaller caller)
    {
        AlarmRuleService = new AlarmRuleService(caller);
        AlarmHistoryService = new AlarmHistoryService(caller);
    }

    public IAlarmRuleService AlarmRuleService { get; }

    public IAlarmHistoryService AlarmHistoryService { get; }
}
