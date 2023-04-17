// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests.Extensions;

public static class ModelBuilderExtensions
{
    public static void InitializeStudentConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HobbyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StudentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GoodsEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new Goods2EntityTypeConfiguration());
    }
}
