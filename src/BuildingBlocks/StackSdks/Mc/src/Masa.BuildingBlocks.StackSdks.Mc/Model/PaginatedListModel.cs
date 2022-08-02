// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class PaginatedListModel<T>
{
    public long Total { get; set; }

    public int TotalPages { get; set; }

    public List<T> Result { get; set; } = default!;

    public PaginatedListModel()
    {
        Result = new List<T>();
    }

    public PaginatedListModel(long total, int totalPages, List<T> result)
    {
        Total = total;
        TotalPages = totalPages;
        Result = result;
    }
}
