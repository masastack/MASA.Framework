// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers;

public class CustomHeaderCaller : HttpClientCallerBase
{
    private readonly TokenProvider _tokenProvider;

    public CustomHeaderCaller(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override string BaseAddress { get; set; } = "https://github.com/masastack";

    public async Task<bool> GetAsync()
    {
        var res = await Caller.GetAsync("");
        return res.IsSuccessStatusCode && res.StatusCode == HttpStatusCode.OK;
    }

    protected override void ConfigHttpRequestMessage(HttpRequestMessage requestMessage)
    {
        Assert.IsTrue(_tokenProvider.Token == "token");
    }
}
