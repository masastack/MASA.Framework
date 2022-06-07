// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class IdentityResourceRepository : Repository<OidcDbContext, IdentityResource, int>, IIdentityResourceRepository
{
    public IdentityResourceRepository(OidcDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public override async ValueTask<IdentityResource> AddAsync(IdentityResource entity, CancellationToken cancellationToken = default)
    {
        return await base.AddAsync(entity, cancellationToken);
    }

    public async Task<IdentityResource?> GetDetailAsync(int id)
    {
        var idrs = await Context.Set<IdentityResource>()
                                .Include(idrs => idrs.UserClaims)
                                .Include(idrs => idrs.Properties)
                                .FirstOrDefaultAsync(idrs => idrs.Id == id);

        return idrs;
    }
}
