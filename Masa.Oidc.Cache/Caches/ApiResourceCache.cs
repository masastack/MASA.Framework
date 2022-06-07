// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.Cache.Caches;

public class ApiResourceCache : IApiResourceCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ApiResourceCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ApiResourceModel>> GetListAsync()
    {
        var ApiResources = await _memoryCacheClient.GetAsync<List<ApiResourceModel>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        return ApiResources;
    }

    public async Task AddOrUpdateAsync(ApiResource apiResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{apiResource.Id}";
        await _memoryCacheClient.SetAsync(key, new ApiResourceModel(apiResource.Name, apiResource.DisplayName, apiResource.UserClaims.Select(uc => uc.UserClaim.Name).ToList())
        {
            Scopes = apiResource.ApiScopes.Select(a => a.ApiScope.Name).ToList(),
            ApiSecrets = apiResource.Secrets.Select(s => new SecretModel(s.Value,s.Description,s.Expiration)).ToList(),
            AllowedAccessTokenSigningAlgorithms = Convert(apiResource.AllowedAccessTokenSigningAlgorithms),
            Enabled = apiResource.Enabled,
            Description = apiResource.Description,
            ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument,
        });

        ICollection<string> Convert(string sourceMember)
        {
            var list = new HashSet<string>();
            if (!String.IsNullOrWhiteSpace(sourceMember))
            {
                sourceMember = sourceMember.Trim();
                foreach (var item in sourceMember.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct())
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }

    public async Task RemoveAsync(ApiResource apiResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{apiResource.Id}";
        await _memoryCacheClient.RemoveAsync<ApiResourceModel>(key);
    }
}
