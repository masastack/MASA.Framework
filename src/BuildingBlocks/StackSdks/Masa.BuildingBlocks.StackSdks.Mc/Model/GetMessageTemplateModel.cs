// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class GetMessageTemplateModel : PaginatedOptions
{
    public string Filter { get; set; } = string.Empty;

    public MessageTemplateStates? Status { get; set; }

    public MessageTemplateAuditStatuses? AuditStatus { get; set; }

    public ChannelTypes? ChannelType { get; set; }

    public Guid ChannelId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int TemplateType { get; set; }
}
