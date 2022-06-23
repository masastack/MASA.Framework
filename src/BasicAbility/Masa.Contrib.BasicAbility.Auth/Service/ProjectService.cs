// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class ProjectService : IProjectService
{
    readonly ICallerProvider _callerProvider;
    readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;

    const string PARTY = "api/project/";

    public ProjectService(ICallerProvider callerProvider, IMultiEnvironmentUserContext multiEnvironmentUserContext)
    {
        _callerProvider = callerProvider;
        _multiEnvironmentUserContext = multiEnvironmentUserContext;
    }

    public async Task<List<ProjectModel>> GetGlobalNavigations()
    {
        var userId = _multiEnvironmentUserContext.GetUserId<Guid>();
        var environment = _multiEnvironmentUserContext.Environment ?? "";
        var requestUri = $"{PARTY}navigations?userId={userId}&environment={environment}";
        return await _callerProvider.GetAsync<List<ProjectModel>>(requestUri) ?? new();
    }
}
