// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Mc.Model;

public class PaginatedOptionsModel
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string Sorting { get; set; }

    public PaginatedOptionsModel(string sorting = "", int page = 1, int pageSize = 10)
    {
        Sorting = sorting;
        Page = page;
        PageSize = pageSize;
    }
}
