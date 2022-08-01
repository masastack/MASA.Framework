// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc;

public class McClient: IMcClient
{
    public McClient(ICaller caller)
    {
        ChannelService = new ChannelService(caller);
        MessageTaskService = new MessageTaskService(caller);
        MessageTemplateService = new MessageTemplateService(caller);
        ReceiverGroupService = new ReceiverGroupService(caller);
        WebsiteMessageService = new WebsiteMessageService(caller);
    }

    public IChannelService ChannelService { get; }

    public IMessageTaskService MessageTaskService { get; }

    public IMessageTemplateService MessageTemplateService { get; }

    public IReceiverGroupService ReceiverGroupService { get; }

    public IWebsiteMessageService WebsiteMessageService { get; }
}
