// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public static class MasaDbContextBuilderExtensions
{
    public static IMasaDbContextBuilder UseFilter(
        this IMasaDbContextBuilder masaDbContextBuilder,
        Action<FilterOptions>? options = null)
        => masaDbContextBuilder.UseFilterCore(options);

    private static IMasaDbContextBuilder UseFilterCore(
        this IMasaDbContextBuilder masaDbContextBuilder,
        Action<FilterOptions>? options = null)
    {
        var filterOptions = new FilterOptions();
        options?.Invoke(filterOptions);

        masaDbContextBuilder.Services.TryAddScoped(typeof(DataFilter<>));
        masaDbContextBuilder.Services.TryAddScoped<IDataFilter, DataFilter>();

        masaDbContextBuilder.EnableSoftDelete = filterOptions.EnableSoftDelete;

        return masaDbContextBuilder;
    }
}
