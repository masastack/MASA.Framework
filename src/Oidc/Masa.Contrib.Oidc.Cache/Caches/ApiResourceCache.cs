// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.Cache.Caches;

public class ApiResourceCache : IApiResourceCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ApiResourceCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ApiResourceModel>> GetListAsync(IEnumerable<string> names)
    {
        var keys = names.Select(name => $"{CacheKeyConstants.API_RESOURCE_KEY}_{name}");
        var apiResources = await _memoryCacheClient.GetListAsync<ApiResourceModel>(keys.ToArray()) ?? new List<ApiResourceModel>();
        return apiResources.Where(i => i is not null).ToList()!;
    }

    public async Task<List<ApiResourceModel>> GetListAsync()
    {
        var apiResources = await _memoryCacheClient.GetAsync<List<ApiResourceModel>>(CacheKeyConstants.API_RESOURCE_KEY) ?? new();
        return apiResources;
    }

    public async Task AddOrUpdateAsync(ApiResource apiResource)
    {
        string key = $"{CacheKeyConstants.API_RESOURCE_KEY}_{apiResource.Id}";
        await _memoryCacheClient.SetAsync(key, new ApiResourceModel(apiResource.Name, apiResource.DisplayName, apiResource.UserClaims.Select(uc => uc.UserClaim.Name).ToList())
        {
            Scopes = apiResource.ApiScopes.Select(a => a.ApiScope.Name).ToList(),
            ApiSecrets = apiResource.Secrets.Select(s => new SecretModel(s.Value, s.Description, s.Expiration)).ToList(),
            AllowedAccessTokenSigningAlgorithms = Convert(apiResource.AllowedAccessTokenSigningAlgorithms),
            Enabled = apiResource.Enabled,
            Description = apiResource.Description,
            ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument,
        });

        ICollection<string> Convert(string sourceMember)
        {
            var list = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(sourceMember))
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
        string key = $"{CacheKeyConstants.API_RESOURCE_KEY}_{apiResource.Id}";
        await _memoryCacheClient.RemoveAsync<ApiResourceModel>(key);
    }

    public async Task AddAllAsync(List<ApiResource> apiResources)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.API_RESOURCE_KEY, apiResources);
    }
}
