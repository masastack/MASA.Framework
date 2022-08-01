// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Model;

public class MessageTemplateModel : AuditEntityModel<Guid, Guid>
{
    public Guid ChannelId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Markdown { get; set; } = string.Empty;

    public string TemplateId { get; set; } = string.Empty;

    public bool IsJump { get; set; }

    public string JumpUrl { get; set; } = string.Empty;

    public string Sign { get; set; } = string.Empty;

    public MessageTemplateStates Status { get; set; }

    public MessageTemplateAuditStatuses AuditStatus { get; set; }

    public DateTimeOffset? AuditTime { get; set; }

    public DateTimeOffset? InvalidTime { get; set; }

    public string AuditReason { get; set; } = string.Empty;

    public int TemplateType { get; set; }

    public string TemplateTypeDisplayName { get; set; }

    public long PerDayLimit { get; set; }

    public bool IsStatic { get; set; }

    public List<MessageTemplateItemModel> Items { get; set; }

    public MessageTemplateModel()
    {
        this.Items = new List<MessageTemplateItemModel>();
    }
}
