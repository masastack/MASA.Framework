// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.DynamicsCrm.EntityFrameworkCore;

public static class MasaDynamicsCrmDbModelBuilderExtensions
{
    private static IEnumerable<IMutableEntityType> EntityTypes(this ModelBuilder builder)
    {
        return builder.Model.GetEntityTypes();
    }

    public static IEnumerable<IMutableEntityType> EntityTypes<T>(this ModelBuilder builder)
    {
        return builder.Model.GetEntityTypes().Where(x => typeof(T).IsAssignableFrom(x.ClrType));
    }

    public static IEnumerable<IMutableProperty> Properties(this ModelBuilder builder)
    {
        return builder.EntityTypes().SelectMany(entityType => entityType.GetProperties());
    }

    public static IEnumerable<IMutableProperty> Properties<T>(this ModelBuilder builder)
    {
        return builder.EntityTypes().SelectMany(entityType => entityType.GetProperties().Where(x => typeof(T).IsAssignableFrom(x.ClrType)));
    }

    public static void Configure(this IEnumerable<IMutableEntityType> entityTypes, Action<IMutableEntityType> convention)
    {
        foreach (var entityType in entityTypes)
        {
            convention(entityType);
        }
    }

    public static void Configure(this IEnumerable<IMutableProperty> propertyTypes, Action<IMutableProperty> convention)
    {
        foreach (var propertyType in propertyTypes)
        {
            convention(propertyType);
        }
    }
}
