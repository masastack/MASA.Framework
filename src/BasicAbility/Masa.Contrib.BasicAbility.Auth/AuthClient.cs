// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth;

public class AuthClient : IAuthClient
{
    public AuthClient(ICallerProvider callerProvider, IMultiEnvironmentUserContext userContext)
    {
        UserService = new UserService(callerProvider, userContext);
        SubjectService = new SubjectService(callerProvider);
        TeamService = new TeamService(callerProvider, userContext);
        ProjectService = new ProjectService(callerProvider, userContext);
        PermissionService = new PermissionService(callerProvider, userContext);
    }

    public IUserService UserService { get; }

    public ISubjectService SubjectService { get; }

    public ITeamService TeamService { get; }

    public IPermissionService PermissionService { get; }

    public IProjectService ProjectService { get; }
}

