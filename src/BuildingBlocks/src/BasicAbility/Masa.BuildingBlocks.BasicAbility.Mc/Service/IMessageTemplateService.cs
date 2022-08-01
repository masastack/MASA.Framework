// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Service;

public interface IMessageTemplateService
{
    Task<MessageTemplateModel?> GetAsync(Guid id);

    Task<PaginatedListModel<MessageTemplateModel>> GetListAsync(GetMessageTemplateModel options);
}
