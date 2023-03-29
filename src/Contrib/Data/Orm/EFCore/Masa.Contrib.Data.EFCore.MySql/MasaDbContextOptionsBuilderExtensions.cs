// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseMySQL(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseMySQL(connectionString, mySqlOptionsAction);

        ConnectionStringConfigProvider.ConnectionStrings.AddOrUpdate(builder.DbContextType, _ => connectionString);

        return builder;
    }

    public static MasaDbContextOptionsBuilder UseMySQL(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<MySQLDbContextOptionsBuilder>? mySqlOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseMySQL(connection, mySqlOptionsAction);

        ConnectionStringConfigProvider.ConnectionStrings.AddOrUpdate(builder.DbContextType, _ => connection.ConnectionString);

        return builder;
    }
}
