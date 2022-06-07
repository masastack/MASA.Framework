// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class ApiScopeRepository : Repository<OidcDbContext, ApiScope, int>, IApiScopeRepository
{
    public ApiScopeRepository(OidcDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<ApiScope?> GetDetailAsync(int id)
    {
        var apiScope = await Context.Set<ApiScope>()
                                .Include(apiScope => apiScope.UserClaims)
                                .Include(apiScope => apiScope.Properties)
                                .FirstOrDefaultAsync(apiScope => apiScope.Id == id);

        return apiScope;
    }
}
