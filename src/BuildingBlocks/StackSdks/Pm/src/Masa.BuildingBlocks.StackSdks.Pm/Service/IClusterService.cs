// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Pm.Model;

namespace Masa.BuildingBlocks.StackSdks.Pm.Service;

public interface IClusterService
{
    Task<List<ClusterModel>> GetListAsync();

    Task<List<ClusterModel>> GetListByEnvIdAsync(int envId);

    Task<ClusterDetailModel> GetAsync(int Id);

    Task<List<EnvironmentClusterModel>> GetEnvironmentClustersAsync();
}
