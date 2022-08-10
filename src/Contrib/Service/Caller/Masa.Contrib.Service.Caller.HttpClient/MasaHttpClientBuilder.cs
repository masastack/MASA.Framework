// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient;

public class MasaHttpClientBuilder
{
    [Obsolete($"recommended to use {nameof(BaseAddress)}, {nameof(BaseApi)} has expired")]
    public string BaseApi { get => BaseAddress; set => BaseAddress = value; }

    public string BaseAddress { get; set; }

    public string Prefix { get; set; }

    public Action<System.Net.Http.HttpClient>? Configure { get; set; }

    public MasaHttpClientBuilder() : this(null)
    {
    }

    public MasaHttpClientBuilder(Action<System.Net.Http.HttpClient>? configure)
        : this(string.Empty, configure)
    {
    }

    public MasaHttpClientBuilder(string baseAddress, Action<System.Net.Http.HttpClient>? configure)
        : this(baseAddress, string.Empty, configure)
    {
    }

    public MasaHttpClientBuilder(string baseAddress, string prefix, Action<System.Net.Http.HttpClient>? configure)
    {
        BaseAddress = baseAddress;
        Prefix = prefix;
        Configure = configure;
    }

    public virtual void ConfigureHttpClient(System.Net.Http.HttpClient httpClient)
    {
        if (!string.IsNullOrEmpty(BaseAddress))
            httpClient.BaseAddress = new Uri(BaseAddress);

        Configure?.Invoke(httpClient);
    }
}
