// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;

public sealed class PaginationDto<TModel>
{
    public long Total { get; }

    public List<TModel> Items { get; }

    public PaginationDto()
    {
        Items = new List<TModel>();
    }

    [JsonConstructor]
    public PaginationDto(long total, List<TModel> items)
    {
        Total = total;
        Items = items;
    }
}
