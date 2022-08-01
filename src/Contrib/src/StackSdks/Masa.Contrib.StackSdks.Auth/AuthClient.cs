// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Auth.Service;

namespace Masa.Contrib.StackSdks.Auth;

public class AuthClient : IAuthClient
{
    public AuthClient(ICaller caller, IMultiEnvironmentUserContext userContext)
    {
        UserService = new UserService(caller, userContext);
        SubjectService = new SubjectService(caller);
        TeamService = new TeamService(caller, userContext);
        ProjectService = new ProjectService(caller, userContext);
        PermissionService = new PermissionService(caller, userContext);
    }

    public IUserService UserService { get; }

    public ISubjectService SubjectService { get; }

    public ITeamService TeamService { get; }

    public IPermissionService PermissionService { get; }

    public IProjectService ProjectService { get; }
}

