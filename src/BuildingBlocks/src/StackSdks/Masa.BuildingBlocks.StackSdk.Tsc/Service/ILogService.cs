// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Service;

public interface ILogService
{
    Task<IEnumerable<string>> GetFieldsAsync();

    Task<IEnumerable<KeyValuePair<string, string>>> GetAggregationAsync(LogAggregationRequest query);

    Task<object> GetLatestAsync(LogLatestRequest query);
}
