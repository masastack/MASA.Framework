// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Pm.Service;

public interface IProjectService
{
    Task<List<ProjectAppsModel>> GetProjectAppsAsync(string envName);

    Task<List<ProjectModel>> GetListByEnvironmentClusterIdAsync(int envClusterId);

    Task<List<ProjectModel>> GetListByTeamIdsAsync(List<Guid> teamIds);

    Task<ProjectDetailModel> GetAsync(int id);

    Task<ProjectDetailModel> GetByIdentityAsync(string identity);

    Task<List<ProjectTypeModel>> GetProjectTypesAsync();
}
