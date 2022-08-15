// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient;

public abstract class HttpClientCallerBase : CallerBase
{
    protected abstract string BaseAddress { get; set; }

    protected virtual string Prefix { get; set; } = string.Empty;

    public HttpClientCallerBase()
    {

    }

    protected HttpClientCallerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override void UseCallerExtension() => UseHttpClient();

    protected virtual IHttpClientBuilder UseHttpClient()
    {
        return CallerOptions.UseHttpClient(Name!, httpClientBuilder =>
        {
            httpClientBuilder.Prefix = Prefix;
            httpClientBuilder.BaseAddress = BaseAddress;
            httpClientBuilder.Configure = ConfigureHttpClient;
        });
    }

    protected virtual void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
    }
}
