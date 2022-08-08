// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class ProjectService : IProjectService
{
    readonly ICaller _caller;
    readonly IUserContext _userContext;

    const string PARTY = "api/project/";

    public ProjectService(ICaller caller, IUserContext userContext)
    {
        _caller = caller;
        _userContext = userContext;
    }

    public async Task<List<ProjectModel>> GetGlobalNavigations()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{PARTY}navigations?userId={userId}";
        return await _caller.GetAsync<List<ProjectModel>>(requestUri) ?? new();
    }
}
