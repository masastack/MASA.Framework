// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public abstract class MasaDbContext : DefaultMasaDbContext<Guid>
{
    protected MasaDbContext() : base(new MasaDbContextOptions<MasaDbContext>())
    {
    }

    public MasaDbContext(MasaDbContextOptions options) : base(options)
    {
    }
}

public abstract class MasaDbContext<TDbContext> : MasaDbContext
    where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
{
    protected MasaDbContext() : base()
    {
    }

    public MasaDbContext(MasaDbContextOptions<TDbContext> options) : base(options)
    {
    }
}

public abstract class MasaDbContext<TDbContext, TMultiTenantId> : DefaultMasaDbContext<TMultiTenantId>
    where TDbContext : DefaultMasaDbContext<TMultiTenantId>, IMasaDbContext
    where TMultiTenantId : IComparable
{
    protected MasaDbContext() : base()
    {
    }

    public MasaDbContext(MasaDbContextOptions<TDbContext> options) : base(options)
    {
    }
}
