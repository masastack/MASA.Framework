// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient;

[ExcludeFromCodeCoverage]
public class MasaHttpClientBuilder : IMasaCallerClientBuilder, IHttpClientBuilder
{
    public string Name { get; }

    public IServiceCollection Services { get; }

    public MasaHttpClientBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }
}
