// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient;

public abstract class HttpClientCallerBase : CallerBase
{
    protected abstract string BaseAddress { get; set; }

    protected virtual string Prefix { get; set; } = string.Empty;

    protected HttpClientCallerBase()
    {

    }

    protected HttpClientCallerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override void UseCallerExtension() => UseHttpClient();

    protected virtual void UseHttpClientPre()
    {
    }

    private MasaHttpClientBuilder UseHttpClient()
    {
        UseHttpClientPre();

        var masaHttpClientBuilder = CallerBuilder.UseHttpClient(callerClient =>
        {
            callerClient.Prefix = Prefix;
            callerClient.BaseAddress = BaseAddress;
            callerClient.Configure = ConfigureHttpClient;
            ConfigMasaCallerClient(callerClient);
        });

        UseHttpClientPost(masaHttpClientBuilder);

        return masaHttpClientBuilder;
    }

    protected virtual void UseHttpClientPost(MasaHttpClientBuilder masaHttpClientBuilder)
    {

    }

    protected virtual void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
    }
}
