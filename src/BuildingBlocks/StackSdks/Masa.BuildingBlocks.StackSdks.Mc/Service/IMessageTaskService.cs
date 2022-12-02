// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Service;

public interface IMessageTaskService
{
    Task<MessageTaskModel?> GetAsync(Guid id);

    Task SendOrdinaryMessageByInternalAsync(SendOrdinaryMessageByInternalModel options);

    Task SendTemplateMessageByInternalAsync(SendTemplateMessageByInternalModel options);

    Task SendOrdinaryMessageByExternalAsync(SendOrdinaryMessageByExternalModel options);

    Task SendTemplateMessageByExternalAsync(SendTemplateMessageByExternalModel options);
}
