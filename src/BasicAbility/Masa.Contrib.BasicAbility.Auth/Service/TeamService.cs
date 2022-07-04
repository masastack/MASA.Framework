// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class TeamService : ITeamService
{
    readonly ICallerProvider _callerProvider;
    readonly string _party = "api/team/";
    readonly IUserContext _userContext;

    public TeamService(ICallerProvider callerProvider, IUserContext userContext)
    {
        _callerProvider = callerProvider;
        _userContext = userContext;
    }

    public async Task<TeamDetailModel?> GetDetailAsync(Guid id)
    {
        var requestUri = $"{_party}detail";
        return await _callerProvider.GetAsync<object, TeamDetailModel>(requestUri, new { id });
    }

    public async Task<List<TeamModel>> GetListAsync(Guid userId = default)
    {
        var requestUri = $"{_party}list";
        if (Guid.Empty != userId)
        {
            requestUri = $"{requestUri}?userId={userId}";
        }
        return await _callerProvider.GetAsync<List<TeamModel>>(requestUri) ?? new();
    }

    public async Task<List<TeamModel>> GetAllAsync()
    {
        var requestUri = $"{_party}list";
        return await _callerProvider.GetAsync<List<TeamModel>>(requestUri) ?? new();
    }

    public async Task<List<TeamModel>> GetUserTeamsAsync()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{_party}list?userId={userId}";
        return await _callerProvider.GetAsync<List<TeamModel>>(requestUri) ?? new();
    }
}

