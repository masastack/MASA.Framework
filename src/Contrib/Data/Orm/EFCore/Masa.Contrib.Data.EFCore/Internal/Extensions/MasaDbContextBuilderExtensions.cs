// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Cosmos")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.InMemory")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.MySql")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Oracle")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Pomelo.MySql")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.PostgreSql")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.Sqlite")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore.SqlServer")]

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

internal static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder ConfigMasaDbContextAndConnectionStringRelations(
        this MasaDbContextBuilder builder,
        string connectionString)
    {
        var name = ConnectionStringNameAttribute.GetConnStringName(builder.DbContextType);

        builder.Services.Configure<MasaDbConnectionOptions>(masaDbConnectionOptions =>
        {
            if (masaDbConnectionOptions.ConnectionStrings.ContainsKey(name) &&
                connectionString != masaDbConnectionOptions.ConnectionStrings.GetConnectionString(name))
                throw new ArgumentException($"The [{builder.DbContextType.Name}] Database Connection String already exists");

            masaDbConnectionOptions.TryAddConnectionString(name, connectionString);
        });

        return builder;
    }
}
