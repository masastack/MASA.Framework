// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth;

public class AuthClient : IAuthClient
{
    public AuthClient(ICallerProvider callerProvider)
    {
        UserService = new UserService(callerProvider);
        SubjectService = new SubjectService(callerProvider);
        TeamService = new TeamService(callerProvider);
    }

    public IUserService UserService { get; }

    public ISubjectService SubjectService { get; }

    public ITeamService TeamService { get; }

    public List<SubjectModel> GetList(ICallerProvider callerProvider,string requestUri,string filter)
    {
        var a = callerProvider.GetAsync<object, List<SubjectModel>>(requestUri, new { filter }).Result;
        return a ?? new();
    }
}

