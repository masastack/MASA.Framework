// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public class EFDbContextOptionsBuilder<TDbContext> : EFDbContextOptionsBuilder
    where TDbContext : MasaDbContext, IMasaDbContext
{
    public EFDbContextOptionsBuilder()
        : base(new DbContextOptions<TDbContext>(), false)
    {
    }
}
