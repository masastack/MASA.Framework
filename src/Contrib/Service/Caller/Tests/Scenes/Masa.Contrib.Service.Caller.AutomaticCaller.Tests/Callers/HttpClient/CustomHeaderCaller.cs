// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests.Callers;

public class CustomHeaderCaller : HttpClientCallerBase
{
    private readonly TokenProvider? _tokenProvider;
    protected override string BaseAddress { get; set; } = "https://github.com/masastack";

    public CustomHeaderCaller(TokenProvider? tokenProvider = null) => _tokenProvider = tokenProvider;

    public async Task<bool> GetAsync()
    {
        var res = await Caller.GetAsync("");
        return res is { IsSuccessStatusCode: true, StatusCode: HttpStatusCode.OK };
    }

    protected override Task ConfigHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        Assert.IsTrue(_tokenProvider is { Token: "token" });
        return Task.CompletedTask;
    }
}
