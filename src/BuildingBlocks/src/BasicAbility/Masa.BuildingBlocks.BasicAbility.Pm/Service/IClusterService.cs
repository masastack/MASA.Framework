// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Pm.Service;

public interface IClusterService
{
    Task<List<ClusterModel>> GetListAsync();

    Task<List<ClusterModel>> GetListByEnvIdAsync(int envId);

    Task<ClusterDetailModel> GetAsync(int Id);

    Task<List<EnvironmentClusterModel>> GetEnvironmentClustersAsync();
}
