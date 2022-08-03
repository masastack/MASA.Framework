// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests.Infrastructure.Callers;

public class GithubCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "https://github.com/masastack";

    public GithubCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task<bool> GetAsync()
    {
        var res = await CallerProvider.GetAsync("");
        return res.IsSuccessStatusCode && res.StatusCode == HttpStatusCode.OK;
    }
}
