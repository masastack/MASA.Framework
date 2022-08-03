// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Cache.Caches;

public interface IApiScopeCache
{
    Task<List<ApiScopeModel>> GetListAsync(IEnumerable<string> names);

    Task<List<ApiScopeModel>> GetListAsync();

    Task SetAsync(ApiScope apiScope);

    Task SetRangeAsync(IEnumerable<ApiScope> apiScopes);

    Task RemoveAsync(ApiScope apiScope);

    Task ResetAsync(IEnumerable<ApiScope> identityResources);
}
