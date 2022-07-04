// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Pm.Service;

public class EnvironmentService : IEnvironmentService
{
    private readonly ICallerProvider _callerProvider;

    public EnvironmentService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<EnvironmentDetailModel> GetAsync(int id)
    {
        var requestUri = $"api/v1/env/{id}";
        var result = await _callerProvider.GetAsync<EnvironmentDetailModel>(requestUri);

        return result ?? new();
    }

    public async Task<List<EnvironmentModel>> GetListAsync()
    {
        var requestUri = $"api/v1/env";
        var result = await _callerProvider.GetAsync<List<EnvironmentModel>>(requestUri);

        return result ?? new();
    }
}
