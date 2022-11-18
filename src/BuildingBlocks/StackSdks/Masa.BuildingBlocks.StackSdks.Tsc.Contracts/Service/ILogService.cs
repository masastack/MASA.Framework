// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Service;

public interface ILogService
{
    Task<PaginationDto<LogResponseDto>> ListAsync(BaseRequestDto query);

    Task<IEnumerable<MappingResponseDto>> MappingAsync();

    Task<object> AggregateAsync(SimpleAggregateRequestDto query);
}
