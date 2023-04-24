﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

#pragma warning disable S3011
#pragma warning disable CS8603
#pragma warning disable S1135
internal static class DbContextOptionsExtensions
{
    private static readonly Func<DbContextOptions, ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)>>
        Func = InitializeExtensionsMap();

    static Func<DbContextOptions, ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)>> InitializeExtensionsMap()
    {
        var property =
            typeof(DbContextOptions).GetProperty("ExtensionsMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MasaArgumentException.ThrowIfNull(property);
        var param = Expression.Parameter(typeof(DbContextOptions));
        var body = Expression.Property(param, property);
        var lambda = Expression
            .Lambda<Func<DbContextOptions, ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)>>>(body,
                param);
        return lambda.Compile();
    }

    public static ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)> GetExtensionsMap(
        this DbContextOptions dbContextOptions)
    {
        return Func.Invoke(dbContextOptions);
    }
}
#pragma warning restore S1135
#pragma warning restore CS8603
#pragma warning restore S3011
