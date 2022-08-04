// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class TeamService : ITeamService
{
    readonly ICaller _caller;
    readonly string _party = "api/team/";
    readonly IUserContext _userContext;

    public TeamService(ICaller caller, IUserContext userContext)
    {
        _caller = caller;
        _userContext = userContext;
    }

    public async Task<TeamDetailModel?> GetDetailAsync(Guid id)
    {
        var requestUri = $"{_party}detail";
        return await _caller.GetAsync<object, TeamDetailModel>(requestUri, new { id });
    }

    public async Task<List<TeamModel>> GetListAsync(Guid userId = default)
    {
        var requestUri = $"{_party}list";
        if (Guid.Empty != userId)
        {
            requestUri = $"{requestUri}?userId={userId}";
        }
        return await _caller.GetAsync<List<TeamModel>>(requestUri) ?? new();
    }

    public async Task<List<TeamModel>> GetAllAsync()
    {
        var requestUri = $"{_party}list";
        return await _caller.GetAsync<List<TeamModel>>(requestUri) ?? new();
    }

    public async Task<List<TeamModel>> GetUserTeamsAsync()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{_party}list?userId={userId}";
        return await _caller.GetAsync<List<TeamModel>>(requestUri) ?? new();
    }
}

