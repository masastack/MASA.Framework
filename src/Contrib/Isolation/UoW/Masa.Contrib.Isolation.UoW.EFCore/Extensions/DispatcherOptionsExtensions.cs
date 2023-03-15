// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseIsolationUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
        => options.UseIsolationUoW<TDbContext, Guid>(isolationBuilder, optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IDispatcherOptions UseIsolationUoW<TDbContext, TTenantId>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
        where TTenantId : IComparable
        => options.UseIsolationUoW<TDbContext, TTenantId, TTenantId>(isolationBuilder, optionsBuilder, disableRollbackOnFailure,
            useTransaction);

    public static IDispatcherOptions UseIsolationUoW<TDbContext, TTenantId, TUserId>(
        this IDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
        where TTenantId : IComparable
        where TUserId : IComparable
        => options.UseIsolationUoW<IDispatcherOptions, TDbContext, TTenantId, TUserId>(
            isolationBuilder,
            optionsBuilder,
            disableRollbackOnFailure,
            useTransaction);

#pragma warning disable S2436
    internal static TDispatcherOptions UseIsolationUoW<TDispatcherOptions, TDbContext, TTenantId, TUserId>(
        this TDispatcherOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDispatcherOptions : IDispatcherOptions
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
        where TTenantId : IComparable
        where TUserId : IComparable
    {
        options.Services.UseIsolationUoW<TDbContext, TTenantId>();
        options.UseIsolation(isolationBuilder)
            .UseUoW<TDbContext, TUserId>(optionsBuilder, disableRollbackOnFailure, useTransaction);
        return options;
    }
#pragma warning restore S2436
}
