// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class GoodsService : CustomServiceBase
{
    public GoodsService()
    {
        RouteOptions.Prefix = "api";
        RouteOptions.Version = "v2";
    }

    public GoodsService(IServiceCollection services, string baseUri) : base(services, baseUri)
    {
    }
}
