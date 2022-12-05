// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Utils.Models;

[Obsolete("BasePaginatedList has expired, please use PaginatedListBase")]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class BasePaginatedList<TEntity> : PaginatedListBase<TEntity>
    where TEntity : class
{
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class PaginatedListBase<TEntity>
    where TEntity : class
{
    public long Total { get; set; }

    public int TotalPages { get; set; }

    public List<TEntity> Result { get; set; } = default!;
}
