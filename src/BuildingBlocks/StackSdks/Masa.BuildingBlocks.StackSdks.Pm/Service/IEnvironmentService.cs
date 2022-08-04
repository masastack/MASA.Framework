// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Pm.Model;

namespace Masa.BuildingBlocks.StackSdks.Pm.Service;

public interface IEnvironmentService
{
    Task<List<EnvironmentModel>> GetListAsync();

    Task<EnvironmentDetailModel> GetAsync(int Id);
}
