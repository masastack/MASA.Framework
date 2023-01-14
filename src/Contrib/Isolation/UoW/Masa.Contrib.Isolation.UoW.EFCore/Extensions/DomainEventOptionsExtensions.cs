// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public static class DomainEventOptionsExtensions
{
    public static IDomainEventOptions UseIsolationUoW<TDbContext>(
        this IDomainEventOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        => options.UseIsolationUoW<TDbContext, Guid>(isolationBuilder, optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IDomainEventOptions UseIsolationUoW<TDbContext, TTenantId>(
        this IDomainEventOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TTenantId : IComparable
        => options.UseIsolationUoW<TDbContext, TTenantId, TTenantId>(isolationBuilder, optionsBuilder, disableRollbackOnFailure,
            useTransaction);

    public static IDomainEventOptions UseIsolationUoW<TDbContext, TTenantId, TUserId>(
        this IDomainEventOptions options,
        Action<IsolationBuilder> isolationBuilder,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool useTransaction = true)
        where TDbContext : MasaDbContext, IMasaDbContext
        where TTenantId : IComparable
        where TUserId : IComparable
        => options.UseIsolationUoW<IDomainEventOptions, TDbContext, TTenantId, TUserId>(
            isolationBuilder,
            optionsBuilder,
            disableRollbackOnFailure,
            useTransaction);
}
