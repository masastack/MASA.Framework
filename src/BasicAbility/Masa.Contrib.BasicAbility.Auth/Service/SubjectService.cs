// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class SubjectService : ISubjectService
{
    readonly ICaller _caller;

    public SubjectService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<List<SubjectModel>> GetListAsync(string filter)
    {
        var requestUri = $"api/subject/getList";
        return await _caller.GetAsync<object, List<SubjectModel>>(requestUri, new { filter }) ?? new();
    }
}

