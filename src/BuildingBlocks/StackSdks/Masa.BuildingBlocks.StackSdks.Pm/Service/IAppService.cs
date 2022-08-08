// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Pm.Service;

public interface IAppService
{
    Task<List<AppDetailModel>> GetListAsync();

    Task<List<AppDetailModel>> GetListByProjectIdsAsync(List<int> projectIds);

    Task<AppDetailModel> GetWithEnvironmentClusterAsync(int Id);

    Task<AppDetailModel> GetAsync(int Id);

    Task<AppDetailModel> GetByIdentityAsync(string identity);

    Task<List<AppDetailModel>> GetListByAppTypes(params AppTypes[] appTypes);
}
