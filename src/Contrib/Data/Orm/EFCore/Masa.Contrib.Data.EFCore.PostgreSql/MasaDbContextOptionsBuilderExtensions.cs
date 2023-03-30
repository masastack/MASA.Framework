// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore;

public static class MasaDbContextOptionsBuilderExtensions
{
    public static MasaDbContextOptionsBuilder UseNpgsql(
        this MasaDbContextOptionsBuilder builder,
        string connectionString,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseNpgsql(connectionString, npgsqlOptionsAction);

        return builder;
    }

    public static MasaDbContextOptionsBuilder UseNpgsql(
        this MasaDbContextOptionsBuilder builder,
        DbConnection connection,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        builder.DbContextOptionsBuilder.UseNpgsql(connection, npgsqlOptionsAction);

        return builder;
    }
}
