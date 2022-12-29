// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public abstract class EntityTypeConfigurationBase<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    public bool EnableGenerated { get; }

    public string? Table { get; }

    public string? Schema { get; }

    public EntityTypeConfigurationBase() : this(null, null)
    {
    }

    public EntityTypeConfigurationBase(string? table, string? schema = null)
        : this(null, table, schema)
    {
    }

    public EntityTypeConfigurationBase(bool? enableGenerated, string? table, string? schema)
    {
        EnableGenerated = enableGenerated ?? EntityTypeGlobalConfiguration.SelfIncrementFunc?.Invoke(typeof(TEntity)) ?? true;
        Schema = schema ?? EntityTypeGlobalConfiguration.SchemaFunc?.Invoke(typeof(TEntity));
        Table = table ?? EntityTypeGlobalConfiguration.TableFunc?.Invoke(typeof(TEntity)) ?? GetSpecifyName(typeof(TEntity).Name);
    }

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        if (!Table.IsNullOrWhiteSpace()) builder.Metadata.SetTableName(Table);
        if (!Schema.IsNullOrWhiteSpace()) builder.Metadata.SetSchema(Schema);

        builder.TryConfigureKey(GetSpecifyName(nameof(IEntity<Guid>.Id)), enableGenerated: EnableGenerated);
        builder.TryConfigureConcurrencyStamp(GetSpecifyName(nameof(IHasConcurrencyStamp.RowVersion)));
        builder.TryConfigureSoftDelete(GetSpecifyName(nameof(ISoftDelete.IsDeleted)));
        builder.TryConfigureAuditEntity(new[]
        {
            GetSpecifyName(nameof(IAuditEntity<Guid>.Creator))!,
            GetSpecifyName(nameof(IAuditEntity<Guid>.CreationTime))!,
            GetSpecifyName(nameof(IAuditEntity<Guid>.Modifier))!,
            GetSpecifyName(nameof(IAuditEntity<Guid>.ModificationTime))!
        });

        CustomerConfigure(builder);
    }

    protected abstract void CustomerConfigure(EntityTypeBuilder<TEntity> builder);

    protected static string? GetSpecifyName(string name)
    {
        switch (EntityTypeGlobalConfiguration.DatabaseNamingRules)
        {
            case DatabaseNamingRules.CamelCase:
                return name.ConvertToCamelCase('_');
            case DatabaseNamingRules.LowerCamelCase:
                return name.ConvertToLowerCamelCase('_');
            case DatabaseNamingRules.SnakeCase:
                return name.CamelCaseToSnakeCase();
            case DatabaseNamingRules.LowerSnakeCase:
                return name.CamelCaseToLowerSnakeCase();
            case null:
                return null;
            default:
                throw new NotSupportedException();
        }
    }
}
