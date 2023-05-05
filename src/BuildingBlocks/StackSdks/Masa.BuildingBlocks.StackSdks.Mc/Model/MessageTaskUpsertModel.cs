// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class MessageTaskUpsertModel
{
    public string DisplayName { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public string ChannelCode { get; set; } = string.Empty;

    public ChannelTypes ChannelType { get; set; }

    public MessageTypes EntityType { get; set; }

    public Guid EntityId { get; set; }

    public bool IsDraft { get; set; }

    public bool IsEnabled { get; set; }

    public SendTargets ReceiverType { get; set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; set; } = MessageTaskSelectReceiverTypes.ManualSelection;

    public string Sign { get; set; } = string.Empty;

    public List<MessageTaskReceiverModel> Receivers { get; set; } = new();

    public SendRuleModel SendRules { get; set; } = new();

    public MessageInfoUpsertModel MessageInfo { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    private MessageTaskSources Source { get;} = MessageTaskSources.Sdk;

    public Guid OperatorId { get; set; } = Guid.Empty;

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();
}
