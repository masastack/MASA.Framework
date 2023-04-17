// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests.EntityConfigurations;

public class GoodsEntityTypeConfiguration : IEntityTypeConfiguration<Goods>
{
    public void Configure(EntityTypeBuilder<Goods> builder)
    {
        builder.Property(s => s.Id).IsRequired();
    }
}

public class Goods2EntityTypeConfiguration : IEntityTypeConfiguration<Goods2>
{
    public void Configure(EntityTypeBuilder<Goods2> builder)
    {
        builder.Property(s => s.Id).IsRequired();
    }
}
