// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Service;

public interface IMetricService
{
    Task<IEnumerable<string>> GetNamesAsync(IEnumerable<string>? match);

    Task<Dictionary<string, List<string>>> GetLabelValuesAsync(LableValuesRequest query);

    Task<string> GetValuesAsync(ValuesRequest query);
}
