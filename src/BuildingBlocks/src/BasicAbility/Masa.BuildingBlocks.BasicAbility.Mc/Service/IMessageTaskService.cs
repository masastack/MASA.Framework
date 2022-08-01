// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Service;

public interface IMessageTaskService
{
    Task<MessageTaskModel?> GetAsync(Guid id);

    Task SendTemplateMessageAsync(SendTemplateMessageModel options);

    Task SendOrdinaryMessageAsync(SendOrdinaryMessageModel options);
}
