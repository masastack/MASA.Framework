// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Pm.Service;

public class ClusterService : IClusterService
{
    private readonly ICaller _caller;

    public ClusterService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<ClusterDetailModel> GetAsync(int id)
    {
        var requestUri = $"api/v1/cluster/{id}";
        var result = await _caller.GetAsync<ClusterDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<List<EnvironmentClusterModel>> GetEnvironmentClustersAsync()
    {
        var requestUri = $"api/v1/envClusters";
        var result = await _caller.GetAsync<List<EnvironmentClusterModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ClusterModel>> GetListAsync()
    {
        var requestUri = $"api/v1/cluster";
        var result = await _caller.GetAsync<List<ClusterModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ClusterModel>> GetListByEnvIdAsync(int envId)
    {
        var requestUri = $"api/v1/{envId}/cluster";
        var result = await _caller.GetAsync<List<ClusterModel>>(requestUri);

        return result ?? new();
    }
}
