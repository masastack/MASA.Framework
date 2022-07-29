// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers;

public class GithubCaller : HttpClientCallerBase
{
    public GithubCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        BaseAddress = "https://github.com/masastack";
    }

    protected override string BaseAddress { get; set; }

    public async Task<bool> GetAsync()
    {
        var res = await Caller.GetAsync("");
        return res.IsSuccessStatusCode && res.StatusCode == HttpStatusCode.OK;
    }
}
