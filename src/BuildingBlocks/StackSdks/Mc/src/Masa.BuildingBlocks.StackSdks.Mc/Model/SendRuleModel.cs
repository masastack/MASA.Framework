// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class SendRuleModel
{
    public bool IsCustom { get; set; }

    public string CronExpression { get; set; } = string.Empty;

    public long SendingCount { get; set; }
}
