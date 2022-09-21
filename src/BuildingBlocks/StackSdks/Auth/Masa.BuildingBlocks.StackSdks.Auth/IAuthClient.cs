// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth;

public interface IAuthClient
{
    IUserService UserService { get; }

    ISubjectService SubjectService { get; }

    ITeamService TeamService { get; }

    IPermissionService PermissionService { get; }

    IProjectService ProjectService { get; }

    ICustomLoginService CustomLoginService { get; }

    IThirdPartyIdpService ThirdPartyIdpService { get; }
}

