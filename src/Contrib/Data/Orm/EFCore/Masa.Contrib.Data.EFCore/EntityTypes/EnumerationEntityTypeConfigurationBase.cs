// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public abstract class EnumerationEntityTypeConfigurationBase<TEntity> : EntityTypeConfigurationBase<TEntity>
    where TEntity : Enumeration
{
    public EnumerationEntityTypeConfigurationBase() : this(null, null) { }

    public EnumerationEntityTypeConfigurationBase(string? table, string? schema)
        : base(false, table, schema)
    {
    }

    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        if (!Table.IsNullOrWhiteSpace()) builder.Metadata.SetTableName(Table);
        if (!Schema.IsNullOrWhiteSpace()) builder.Metadata.SetSchema(Schema);

        builder.HasKey(nameof(Enumeration.Id));
        builder
            .Property(e => e.Id)
            .HasColumnName(GetSpecifyName(nameof(Enumeration.Id)))
            .IsRequired();

        builder
            .Property(e => e.Name)
            .HasColumnName(GetSpecifyName(nameof(Enumeration.Name)))
            .IsRequired();

        CustomerConfigure(builder);
    }
}
