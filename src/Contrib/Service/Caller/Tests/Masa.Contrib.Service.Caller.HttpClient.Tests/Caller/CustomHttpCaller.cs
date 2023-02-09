﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Service.Caller.HttpClient.Tests;

public class CustomHttpCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "https://github.com/masastack/MASA.Framework";

    protected override string Prefix { get; set; } = "custom";

    protected override void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
        httpClient.Timeout = TimeSpan.FromHours(1);
    }

    public ICaller GetCaller() => Caller;
}
