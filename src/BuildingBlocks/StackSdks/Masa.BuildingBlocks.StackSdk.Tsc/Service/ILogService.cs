// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Service;

public interface ILogService
{
    Task<IEnumerable<MappingResponseDto>> GetMappingAsync();

    Task<TResult> GetAggregationAsync<TResult>(SimpleAggregateRequestDto query);

    Task<LogResponseDto> GetLatestAsync(LogLatestRequest query);
}
