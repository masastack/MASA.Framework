// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class SendOrdinaryMessageByInternalModel
{
    public string ChannelCode { get; set; } = string.Empty;

    public ChannelTypes ChannelType { get; set; }

    public SendTargets ReceiverType { get; set; }

    public List<InternalReceiverModel> Receivers { get; set; } = new();

    public SendRuleModel SendRules { get; set; } = new();

    public MessageInfoUpsertModel MessageInfo { get; set; } = new();

    public ExtraPropertyDictionary Variables { get; set; } = new();

    public Guid OperatorId { get; set; } = Guid.Empty;
}
