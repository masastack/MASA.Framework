namespace Masa.Contrib.BasicAbility.Pm.Service;

public class ProjectService : IProjectService
{
    private readonly ICallerProvider _callerProvider;

    public ProjectService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<List<ProjectModel>> GetProjectListAsync(string envName)
    {
        var requestUri = $"api/v1/projectwithapps/{envName}";
        var result = await _callerProvider.GetAsync<List<ProjectModel>>(requestUri);

        return result ?? new List<ProjectModel>();
    }
}
