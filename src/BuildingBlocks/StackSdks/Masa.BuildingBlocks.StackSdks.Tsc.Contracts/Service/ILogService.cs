// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Service;

public interface ILogService
{
    Task<PaginatedListBase<LogResponseDto>> ListAsync(BaseRequestDto query);

    Task<IEnumerable<MappingResponseDto>> GetMappingAsync();

    /// <summary>
    /// when query type: Count,Sum,Avg and DistinctCount return type is double, DateHistogram return IEnumerable<KeyValuePair<double, long>> ,GroupBy return IEnumerable<string>, AllValue is true return IEnumerable<KeyValueParir<string,long>>
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<object> AggregateAsync(SimpleAggregateRequestDto query);
}
