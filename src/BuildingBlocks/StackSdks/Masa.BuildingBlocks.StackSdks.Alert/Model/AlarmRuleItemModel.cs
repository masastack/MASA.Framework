// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Model;

public class AlarmRuleItemModel
{
    public Guid AlarmRuleId { get; set; }

    public string Expression { get; set; } = string.Empty;

    public AlertSeverity AlertSeverity { get; set; }

    public bool IsRecoveryNotification { get; set; }

    public bool IsNotification { get; set; }

    public NotificationConfigModel RecoveryNotificationConfig { get; set; } = new();

    public NotificationConfigModel NotificationConfig { get; set; } = new();
}
