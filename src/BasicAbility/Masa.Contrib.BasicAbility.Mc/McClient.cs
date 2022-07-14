// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc;

public class McClient: IMcClient
{
    public McClient(ICallerProvider callerProvider)
    {
        ChannelService = new ChannelService(callerProvider);
        MessageTaskService = new MessageTaskService(callerProvider);
        MessageTemplateService = new MessageTemplateService(callerProvider);
        ReceiverGroupService = new ReceiverGroupService(callerProvider);
        WebsiteMessageService = new WebsiteMessageService(callerProvider);
        NotificationService = new NotificationService(callerProvider);
    }

    public IChannelService ChannelService { get; }

    public IMessageTaskService MessageTaskService { get; }

    public IMessageTemplateService MessageTemplateService { get; }

    public IReceiverGroupService ReceiverGroupService { get; }

    public IWebsiteMessageService WebsiteMessageService { get; }

    public INotificationService NotificationService { get; }
}
