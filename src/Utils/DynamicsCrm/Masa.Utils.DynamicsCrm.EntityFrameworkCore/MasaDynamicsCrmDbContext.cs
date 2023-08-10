namespace Masa.Utils.DynamicsCrm.EntityFrameworkCore;

public class MasaDynamicsCrmDbContext : MasaDbContext
{
    protected virtual string TableNameSuffix { get; set; } = "Base";

    protected virtual string FieldNamePrefix { get; set; } = "New_";

    protected virtual string TableNamePrefix { get; set; } = "New_";

    protected MasaDynamicsCrmDbContext(MasaDbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        ConfigureDefaultEntity(modelBuilder, TableNameSuffix);
        ConfigureCustomEntity(modelBuilder, TableNameSuffix, FieldNamePrefix, TableNamePrefix);
    }

    protected override void OnBeforeSaveChangesByFilters()
    {
        foreach (var filter in Options!.SaveChangesFilters)
        {
            try
            {
                filter.OnExecuting(ChangeTracker);
            }
            catch (Exception ex)
            {
                throw new MasaException("An error occured when intercept SaveChanges() or SaveChangesAsync()", ex);
            }
        }
    }

    protected virtual void ConfigureDefaultEntity(ModelBuilder modelBuilder, string tableNameSuffix)
    {
        modelBuilder.EntityTypes<CrmEntity>()
            .Where(entity => entity.BaseType == null)
            .Configure(entity => entity.SetTableName(entity.ClrType.Name + tableNameSuffix));

        var crmPreProperties = modelBuilder.Properties()
            .Where(p => p.PropertyInfo != null
                && typeof(CrmEntity).IsAssignableFrom(p.PropertyInfo.ReflectedType)
                && !typeof(CrmFullAuditEntity).IsAssignableFrom(p.PropertyInfo.ReflectedType)
                ).ToList();

        crmPreProperties
            .Where(p => p.Name.Equals("Id"))
            .Configure(prop =>
            {
                prop.SetColumnName(prop.PropertyInfo?.ReflectedType?.Name + prop.PropertyInfo?.Name);
            });

        var crmPrePropertiesName =
            typeof(CrmEntity).GetProperties()
                .Select(x => x.Name).ToList();

        crmPreProperties
            .Where(p => !crmPrePropertiesName.Contains(p.Name))
            .Where(p => !p.Name.Equals("Id"))
            .Where(p => p.FindAnnotation("Relational:ColumnName") == null)
            .Configure(prop => prop.SetColumnName(prop.PropertyInfo?.Name));
    }

    protected virtual void ConfigureCustomEntity(ModelBuilder modelBuilder, string tableNameSuffix, string fieldNamePrefix, string tableNamePrefix)
    {
        modelBuilder.EntityTypes<CrmFullAuditEntity>()
            .Where(entity => entity.BaseType == null)
            .Configure(entity => entity.SetTableName(tableNamePrefix + entity.ClrType.Name + tableNameSuffix));

        var crmCustomPreProperties = modelBuilder.Properties()
            .Where(p =>
                p.PropertyInfo != null && typeof(CrmFullAuditEntity).IsAssignableFrom(p.PropertyInfo.ReflectedType)
                ).ToList();

        crmCustomPreProperties
            .Where(p => p.Name.Equals("Id"))
            .Configure(prop =>
            {
                prop.SetColumnName(fieldNamePrefix + prop.PropertyInfo?.ReflectedType?.Name + prop.PropertyInfo?.Name);
            });

        var crmCustomPrePropertiesName =
            typeof(CrmFullAuditEntity).GetProperties()
                .Select(x => x.Name).ToList();

        crmCustomPreProperties
            .Where(p => !crmCustomPrePropertiesName.Contains(p.Name))
            .Where(p => !p.Name.Equals("Id"))
            .Where(p => p.FindAnnotation("Relational:ColumnName") == null)
            .Configure(prop => prop.SetColumnName(fieldNamePrefix + prop.PropertyInfo?.Name));
    }

    protected override Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = base.CreateFilterExpression<TEntity>();
        if (typeof(ICrmState).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> crmStateCodeFilter = e => ((ICrmState)e).StateCode == 0;

            expression = crmStateCodeFilter.And(expression != null, expression);
        }

        return expression;
    }
}
