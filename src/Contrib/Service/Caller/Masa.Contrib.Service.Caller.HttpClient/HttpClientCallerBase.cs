// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient;

public abstract class HttpClientCallerBase : CallerExpandBase
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

    protected virtual MasaHttpClientBuilder UseHttpClient()
    {
        var masaHttpClientBuilder = CallerOptions.UseHttpClient(callerClient =>
        {
            callerClient.Prefix = Prefix;
            callerClient.BaseAddress = BaseAddress;
            callerClient.Configure = ConfigureHttpClient;
            ConfigMasaCallerClient(callerClient);
        });
        return masaHttpClientBuilder;
    }

    protected virtual void ConfigMasaCallerClient(MasaCallerClient callerClient)
    {
    }

    protected virtual void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
    }
}
