// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Service;

public interface ITraceService
{
    Task<IEnumerable<TraceResponseDto>> GetAsync(string traceId);

    Task<PaginationDto<TraceResponseDto>> ListAsync(BaseRequestDto query);

    Task<object> AggregateAsync(SimpleAggregateRequestDto query);
}
