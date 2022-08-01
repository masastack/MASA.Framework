// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc.Service;

public class LabelService : ILabelService
{
    private readonly IDistributedCacheClient _distributedCacheClient;

    public LabelService(IDistributedCacheClient distributedCacheClient)
    {
        _distributedCacheClient = distributedCacheClient;
    }

    public async Task<List<LabelModel>> GetListByTypeCodeAsync(string typeCode)
    {
        var result = await _distributedCacheClient.GetAsync<List<LabelModel>>(typeCode);

        return result ?? new();
    }
}
