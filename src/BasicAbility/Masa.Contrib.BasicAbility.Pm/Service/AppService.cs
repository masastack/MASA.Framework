// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Pm.Service;

public class AppService : IAppService
{
    private readonly ICaller _caller;

    public AppService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<AppDetailModel> GetAsync(int id)
    {
        var requestUri = $"api/v1/app/{id}";
        var result = await _caller.GetAsync<AppDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<AppDetailModel> GetByIdentityAsync(string identity)
    {
        var requestUri = $"open-api/app/{identity}";
        var result = await _caller.GetAsync<AppDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<List<AppDetailModel>> GetListAsync()
    {
        var requestUri = $"api/v1/app";
        var result = await _caller.GetAsync<List<AppDetailModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<AppDetailModel>> GetListByProjectIdsAsync(List<int> projectIds)
    {
        var requestUri = $"api/v1/projects/app";
        var result = await _caller.PostAsync<List<int>, List<AppDetailModel>>(requestUri, projectIds);

        return result ?? new();
    }

    public async Task<AppDetailModel> GetWithEnvironmentClusterAsync(int id)
    {
        var requestUri = $"api/v1/appWhitEnvCluster/{id}";
        var result = await _caller.GetAsync<AppDetailModel>(requestUri);

        return result ?? new();
    }
}
