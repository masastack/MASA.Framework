namespace Masa.Contrib.BasicAbility.Auth.Service;

public class TeamService : ITeamService
{
    readonly ICallerProvider _callerProvider;

    public TeamService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<TeamDetailModel?> GetDetailAsync(Guid id)
    {
        var requestUri = $"api/team/deatil";
        return await _callerProvider.GetAsync<object, TeamDetailModel>(requestUri, new { id });
    }
}

