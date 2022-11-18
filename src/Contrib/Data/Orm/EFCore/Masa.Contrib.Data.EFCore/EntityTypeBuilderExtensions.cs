// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class EntityTypeBuilderExtensions
{
    private const int MAX_LENGTH = 36;

    public static void TryConfigureConcurrencyStamp(
        this EntityTypeBuilder entityTypeBuilder,
        string? propertyName)
        => entityTypeBuilder.TryConfigureConcurrencyStamp(MAX_LENGTH, propertyName);

    public static void TryConfigureConcurrencyStamp(
        this EntityTypeBuilder entityTypeBuilder,
        int maxLength = MAX_LENGTH,
        string? propertyName = null)
    {
        if (entityTypeBuilder.Metadata.ClrType.IsAssignableTo(typeof(IHasConcurrencyStamp)))
        {
            entityTypeBuilder.Property(nameof(IHasConcurrencyStamp.RowVersion))
                .IsConcurrencyToken()
                .HasMaxLength(maxLength)
                .HasColumnName(propertyName ?? nameof(IHasConcurrencyStamp.RowVersion));
        }
    }
}
