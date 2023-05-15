// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Alert.Model;

public class NotificationConfigModel
{
    public string ChannelCode { get; set; } = default!;

    public string TemplateCode { get; set; } = default!;

    public string TemplateName { get; set; } = default!;

    public int ChannelType { get; set; }

    public List<Guid> Receivers { get; set; } = new();
}
