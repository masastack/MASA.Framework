// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
        CustomLoginService = new CustomLoginService(caller);
        ThirdPartyIdpService = new ThirdPartyIdpService(caller);
    }

    public IUserService UserService { get; }

    public ISubjectService SubjectService { get; }

    public ITeamService TeamService { get; }

    public IPermissionService PermissionService { get; }

    public IProjectService ProjectService { get; }

    public ICustomLoginService CustomLoginService { get; }

    public IThirdPartyIdpService ThirdPartyIdpService { get; }
}

