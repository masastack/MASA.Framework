// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient.Tests;

public class CustomXmlHttpCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "https://github.com/masastack/MASA.Framework";

    protected override void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
        httpClient.Timeout = TimeSpan.FromHours(2);
    }

    protected override void ConfigMasaCallerClient(MasaCallerClient callerClient)
    {
        callerClient.UseXml();
    }

    public ICaller GetCaller() => Caller;
}
