// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Service;

public interface IReceiverGroupService
{
    Task<ReceiverGroupModel?> GetAsync(Guid id);

    Task<PaginatedListModel<ReceiverGroupModel>> GetListAsync(GetReceiverGroupModel inputDto);
}
