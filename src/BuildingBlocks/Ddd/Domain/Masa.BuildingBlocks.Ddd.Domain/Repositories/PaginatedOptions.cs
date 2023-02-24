// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Repositories;

public class PaginatedOptions : RequestPageBase
{
    public Dictionary<string, bool>? Sorting { get; set; }

    public PaginatedOptions()
    {
    }

    public PaginatedOptions(int page, int pageSize, Dictionary<string, bool>? sorting = null)
    {
        Page = page;
        PageSize = pageSize;
        Sorting = sorting;
    }

    /// <summary>
    /// Initialize a new instance of PaginatedOptions.
    /// </summary>
    /// <param name="page">page number</param>
    /// <param name="pageSize">returns per page</param>
    /// <param name="sortField">sort field name</param>
    /// <param name="isDescending">true descending order, false ascending order, default: true</param>
    public PaginatedOptions(int page, int pageSize, string sortField, bool isDescending = true)
        : this(page, pageSize, new Dictionary<string, bool>(new List<KeyValuePair<string, bool>> { new(sortField, isDescending) }))
    {
    }
}
