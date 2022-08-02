// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc;

public interface IMcClient
{
    IChannelService ChannelService { get; }

    IMessageTaskService MessageTaskService { get; }

    IMessageTemplateService MessageTemplateService { get; }

    IReceiverGroupService ReceiverGroupService { get; }

    IWebsiteMessageService WebsiteMessageService { get; }
}
