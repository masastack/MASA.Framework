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
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        => options.UseUoW<TDbContext, Guid>(optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IDispatcherOptions UseUoW<TDbContext, TUserId>(
        this IDispatcherOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
        => options.UseUoW<IDispatcherOptions, TDbContext, TUserId>(optionsBuilder, disableRollbackOnFailure, useTransaction);

    internal static TDispatcherOptions UseUoW<TDispatcherOptions, TDbContext, TUserId>(
        this TDispatcherOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDispatcherOptions : IDispatcherOptions
        where TDbContext : MasaDbContext, IMasaDbContext
        where TUserId : IComparable
    {
        options.Services.UseUoW<TDbContext, TUserId>(nameof(options.Services), optionsBuilder, disableRollbackOnFailure, useTransaction);
        return options;
    }
}
