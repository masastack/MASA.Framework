// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore;

public static class EntityTypeBuilderExtensions
{
    private const int _maxLength = 36;

    public static void TryConfigureConcurrencyStamp(
        this EntityTypeBuilder entityTypeBuilder,
        string? propertyName)
        => entityTypeBuilder.TryConfigureConcurrencyStamp(_maxLength, propertyName);

    public static void TryConfigureConcurrencyStamp(
        this EntityTypeBuilder entityTypeBuilder,
        int maxLength = _maxLength,
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
