// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public abstract class EFDbContextOptionsBuilder
{
    public readonly DbContextOptionsBuilder DbContextOptionsBuilder;

    internal bool EnableSoftDelete { get; private set; }

    protected EFDbContextOptionsBuilder(DbContextOptions options, bool enableSoftDelete)
    {
        DbContextOptionsBuilder = new DbContextOptionsBuilder(options);
        EnableSoftDelete = enableSoftDelete;
    }

    public EFDbContextOptionsBuilder UseSoftDelete()
    {
        EnableSoftDelete = true;
        return this;
    }
}
