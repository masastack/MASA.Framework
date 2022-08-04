// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.HttpClient;

public class MasaHttpClientBuilder
{
    private string _name = default!;

    public string Name
    {
        get => _name;
        set
        {
            if (value is null)
                throw new ArgumentNullException(Name);

            _name = value;
        }
    }

    [Obsolete($"recommended to use {nameof(BaseAddress)}, {nameof(BaseApi)} has expired")]
    public string BaseApi { get => BaseAddress; set => BaseAddress = value; }

    public string BaseAddress { get; set; }

    public string Prefix { get; set; }

    public bool IsDefault { get; set; } = false;

    public Action<System.Net.Http.HttpClient>? Configure { get; set; }

    public MasaHttpClientBuilder() : this("http", null)
    {
    }

    public MasaHttpClientBuilder(string name, Action<System.Net.Http.HttpClient>? configure)
        : this(name, string.Empty, configure)
    {
    }

    public MasaHttpClientBuilder(string name, string baseAddress, Action<System.Net.Http.HttpClient>? configure)
        : this(name, baseAddress, string.Empty, configure)
    {
    }

    public MasaHttpClientBuilder(string name, string baseAddress, string prefix, Action<System.Net.Http.HttpClient>? configure)
    {
        Name = name;
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
