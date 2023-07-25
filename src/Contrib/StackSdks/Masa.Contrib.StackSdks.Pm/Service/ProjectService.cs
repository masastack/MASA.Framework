// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Pm.Service;

public class ProjectService : IProjectService
{
    private readonly ICaller _caller;

    public ProjectService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<List<ProjectAppsModel>> GetProjectAppsAsync(string envName)
    {
        var requestUri = $"open-api/projectwithapps/{envName}";
        var result = await _caller.GetAsync<List<ProjectAppsModel>>(requestUri);

        return result ?? new();
    }

    public async Task<ProjectDetailModel> GetAsync(int id)
    {
        var requestUri = $"api/v1/project/{id}";
        var result = await _caller.GetAsync<ProjectDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<ProjectDetailModel> GetByIdentityAsync(string identity)
    {
        var requestUri = $"open-api/project/{identity}";
        var result = await _caller.GetAsync<ProjectDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<List<ProjectModel>> GetListByEnvironmentClusterIdAsync(int envClusterId)
    {
        var requestUri = $"api/v1/{envClusterId}/project";
        var result = await _caller.GetAsync<List<ProjectModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ProjectModel>> GetListByTeamIdsAsync(List<Guid> teamIds, string environment)
    {
        var requestUri = $"open-api/project/teamProjects/{environment}";
        var result = await _caller.PostAsync<List<ProjectModel>>(requestUri, teamIds);

        return result ?? new();
    }

    public async Task<List<ProjectTypeModel>> GetProjectTypesAsync()
    {
        var requestUri = $"api/v1/project/projectType";
        var result = await _caller.GetAsync<List<ProjectTypeModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ProjectModel>> GetListAsync()
    {
        var requestUri = $"api/v1/projects";
        var result = await _caller.GetAsync<List<ProjectModel>>(requestUri);

        return result ?? new();
    }
}
