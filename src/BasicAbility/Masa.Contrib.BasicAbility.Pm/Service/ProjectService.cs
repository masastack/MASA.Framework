namespace Masa.Contrib.BasicAbility.Pm.Service;

public class ProjectService : IProjectService
{
    private readonly ICallerProvider _callerProvider;

    public ProjectService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<List<ProjectAppsModel>> GetProjectAppsAsync(string envName)
    {
        var requestUri = $"api/v1/projectwithapps/{envName}";
        var result = await _callerProvider.GetAsync<List<ProjectAppsModel>>(requestUri);

        return result ?? new();
    }

    public async Task<ProjectDetailModel> GetAsync(int Id)
    {
        var requestUri = $"api/v1/project/{Id}";
        var result = await _callerProvider.GetAsync<ProjectDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<List<ProjectModel>> GetListByEnvironmentClusterIdAsync(int envClusterId)
    {
        var requestUri = $"api/v1/{envClusterId}/project";
        var result = await _callerProvider.GetAsync<List<ProjectModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ProjectModel>> GetListByTeamIdAsync(Guid teamId)
    {
        var requestUri = $"api/v1/project/teamProjects/{teamId}";
        var result = await _callerProvider.GetAsync<List<ProjectModel>>(requestUri);

        return result ?? new();
    }

    public async Task<List<ProjectTypeModel>> GetProjectTypesAsync()
    {
        var requestUri = $"api/v1/project/projectType";
        var result = await _callerProvider.GetAsync<List<ProjectTypeModel>>(requestUri);

        return result ?? new();
    }
}
