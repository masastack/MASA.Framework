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
            entityTypeBuilder
                .Property(nameof(IHasConcurrencyStamp.RowVersion))
                .IsConcurrencyToken()
                .HasMaxLength(maxLength)
                .HasColumnName(propertyName ?? nameof(IHasConcurrencyStamp.RowVersion));
        }
    }

    public static void TryConfigureSoftDelete(this EntityTypeBuilder entityTypeBuilder, string? propertyName = null)
    {
        if (entityTypeBuilder.Metadata.ClrType.IsAssignableTo(typeof(ISoftDelete)))
        {
            entityTypeBuilder
                .Property(nameof(ISoftDelete.IsDeleted))
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName(propertyName ?? nameof(ISoftDelete.IsDeleted));
        }
    }

    /// <summary>
    /// Try to configure the primary key
    /// Only supports single primary key
    /// </summary>
    /// <param name="entityTypeBuilder"></param>
    /// <param name="propertyName">attribute name</param>
    /// <param name="enableGenerated">enable auto increment</param>
    public static void TryConfigureKey(
        this EntityTypeBuilder entityTypeBuilder,
        string? propertyName = null,
        bool enableGenerated = true)
    {
        if (entityTypeBuilder.Metadata.ClrType.IsImplementerOfGeneric(typeof(IEntity<>)))
        {
            string name = nameof(IEntity<Guid>.Id);
            var propertyBuilder = entityTypeBuilder
                .Property(name)
                .IsRequired()
                .HasColumnName(propertyName ?? name);
            if (enableGenerated) propertyBuilder.ValueGeneratedOnAdd();
            else propertyBuilder.ValueGeneratedNever();
        }
    }

    public static void TryConfigureAuditEntity(
        this EntityTypeBuilder entityTypeBuilder,
        string[]? propertyNames = null)
    {
        if (entityTypeBuilder.Metadata.ClrType.IsImplementerOfGeneric(typeof(IAuditEntity<>)))
        {
            ConfigureProperty(nameof(IAuditEntity<Guid>.Creator), 0);
            ConfigureProperty(nameof(IAuditEntity<Guid>.CreationTime), 1);
            ConfigureProperty(nameof(IAuditEntity<Guid>.Modifier), 2);
            ConfigureProperty(nameof(IAuditEntity<Guid>.ModificationTime), 3);
        }

        void ConfigureProperty(string name, int index)
        {
            entityTypeBuilder
                .Property(name)
                .IsRequired()
                .HasColumnName(GetColumnName(name, index));
        }

        string GetColumnName(string name, int index)
        {
            if (propertyNames == null)
                return name;

            MasaArgumentException.ThrowIfNotEqual(propertyNames.Length, 4, nameof(propertyNames));
            return propertyNames[index];
        }
    }
}
