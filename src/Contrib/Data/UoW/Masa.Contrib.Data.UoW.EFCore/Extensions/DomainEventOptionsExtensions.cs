// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data.UoW;

public static class DomainEventOptionsExtensions
{
    public static IDomainEventOptions UseUoW<TDbContext>(
        this IDomainEventOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool? useTransaction = null)
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
        => options.UseUoW<TDbContext, Guid>(optionsBuilder, disableRollbackOnFailure, useTransaction);

    public static IDomainEventOptions UseUoW<TDbContext, TUserId>(
        this IDomainEventOptions options,
        Action<MasaDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool? useTransaction = null)
        where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
        where TUserId : IComparable
        => options.UseUoW<IDomainEventOptions, TDbContext, TUserId>(optionsBuilder, disableRollbackOnFailure, useTransaction);
}
