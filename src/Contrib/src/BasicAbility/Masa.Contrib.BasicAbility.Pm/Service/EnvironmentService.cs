// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Pm.Service;

public class EnvironmentService : IEnvironmentService
{
    private readonly ICaller _caller;

    public EnvironmentService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<EnvironmentDetailModel> GetAsync(int id)
    {
        var requestUri = $"api/v1/env/{id}";
        var result = await _caller.GetAsync<EnvironmentDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<List<EnvironmentModel>> GetListAsync()
    {
        var requestUri = $"api/v1/env";
        var result = await _caller.GetAsync<List<EnvironmentModel>>(requestUri);

        return result ?? new();
    }
}
