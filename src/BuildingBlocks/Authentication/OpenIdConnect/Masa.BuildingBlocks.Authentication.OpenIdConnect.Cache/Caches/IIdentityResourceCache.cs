// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Cache.Caches;

public interface IIdentityResourceCache
{
    Task<List<IdentityResourceModel>> GetListAsync(IEnumerable<string> names);

    Task<List<IdentityResourceModel>> GetListAsync();

    Task SetAsync(IdentityResource identityResource);

    Task SetRangeAsync(IEnumerable<IdentityResource> identityResources);

    Task RemoveAsync(IdentityResource identityResource);

    Task ResetAsync(IEnumerable<IdentityResource> identityResources);
}
