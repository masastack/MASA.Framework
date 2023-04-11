// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data.UoW;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool? useTransaction = null)
        where TDbContext : DefaultMasaDbContext, IMasaDbContext
        => options.UseUoW<IDispatcherOptions, TDbContext>(optionsBuilder, disableRollbackOnFailure, useTransaction);

    internal static TDispatcherOptions UseUoW<TDispatcherOptions, TDbContext>(
        this TDispatcherOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool? useTransaction = null)
        where TDispatcherOptions : IDispatcherOptions
        where TDbContext : DefaultMasaDbContext, IMasaDbContext
    {
        options.Services.UseUoW<TDbContext>(nameof(options.Services), optionsBuilder, disableRollbackOnFailure, useTransaction);
        return options;
    }
}
