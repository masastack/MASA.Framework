// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Service;

public interface ITeamService
{
    Task<TeamDetailModel?> GetDetailAsync(Guid id);

    Task<List<TeamModel>> GetAllAsync();

    Task<List<TeamModel>> GetUserTeamsAsync();
}

