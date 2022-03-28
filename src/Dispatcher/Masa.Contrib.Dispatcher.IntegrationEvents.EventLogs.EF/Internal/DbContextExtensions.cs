namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Internal;

internal static class DbContextExtensions
{
    private static readonly ConcurrentDictionary<Type, Tables> TableDictionary = new();

    public static string GetTableName<TEntity>(this DbContext dbContext) where TEntity : class
    {
        var type = typeof(TEntity);
        if (TableDictionary.TryGetValue(type, out Tables? table))
            return table.Name;

        table = dbContext.GetTables<TEntity>();
        TableDictionary.TryAdd(type, table);
        return table.Name;
    }

    public static string GetPropertyName<TEntity>(this DbContext dbContext, string propertyName) where TEntity : class
    {
        var type = typeof(TEntity);
        if (TableDictionary.TryGetValue(type, out Tables? table))
            return table.GetPropertyName(propertyName);

        table = dbContext.GetTables<TEntity>();
        TableDictionary.TryAdd(type, table);
        return table.GetPropertyName(propertyName);
    }

    private static string GetPropertyName(this Tables table, string propertyName)
        => table.Properties.Where(p => p.Name == propertyName).Select(p => p.ColunName).FirstOrDefault()!;

    private static Tables GetTables<TEntity>(this DbContext dbContext) where TEntity : class
    {
        var entityType = GetEntityType<TEntity>(dbContext);
        var tableName = entityType.GetTableName()!;
        var schema = entityType.GetSchema()!;
        List<(string Name, string ColunName)> properties = new List<(string Name, string ColunName)>();
        foreach (var property in entityType.GetProperties())
            properties.Add(new(property.Name, property.GetColumnName(StoreObjectIdentifier.Table(tableName, schema))!));
        return new Tables(tableName, schema, properties);
    }

    private static IEntityType GetEntityType<TEntity>(this DbContext dbContext) where TEntity : class
        => dbContext.Model.FindEntityType(typeof(TEntity))!;
}
