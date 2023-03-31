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
        where TDbContext : DefaultMasaDbContext, IMasaDbContext
        => options.UseUoW<IDomainEventOptions, TDbContext>(optionsBuilder, disableRollbackOnFailure, useTransaction);
}
