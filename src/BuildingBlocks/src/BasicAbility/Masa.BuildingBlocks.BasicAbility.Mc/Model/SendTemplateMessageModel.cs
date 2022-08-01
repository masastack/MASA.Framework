// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Model;

public class SendTemplateMessageModel
{
    public string ChannelCode { get; set; } = string.Empty;

    public ChannelTypes? ChannelType { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public SendTargets ReceiverType { get; set; }

    public string Sign { get; set; } = string.Empty;

    public List<MessageTaskReceiverModel> Receivers { get; set; } = new();

    public SendRuleModel SendRules { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public Guid OperatorId { get; set; }
}
