// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class MessageTaskModel : AuditEntityModel<Guid, Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public ChannelModel Channel { get; set; } = new();

    public MessageTypes EntityType { get; set; }

    public Guid EntityId { get; set; }

    public bool IsDraft { get; set; }

    public bool IsEnabled { get; set; }

    public SendTargets ReceiverType { get; set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; set; }

    public DateTimeOffset? SendTime { get; set; }

    public string Sign { get; set; } = string.Empty;

    public MessageInfoModel MessageInfo { get; set; } = new();

    public List<MessageTaskReceiverModel> Receivers { get; set; } = new();

    public SendRuleModel SendRules { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public string Content { get; set; } = string.Empty;

    public MessageTaskStatuses Status { get; set; }

    public MessageTaskSources Source { get; set; }

    public string ModifierName { get; set; } = string.Empty;

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();
}
