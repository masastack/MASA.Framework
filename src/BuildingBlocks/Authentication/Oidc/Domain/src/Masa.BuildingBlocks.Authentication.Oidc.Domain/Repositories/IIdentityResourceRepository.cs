// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Repositories;

public interface IIdentityResourceRepository : IRepositoryBase<IdentityResource>
{
    Task AddStandardIdentityResourcesAsync();
}
