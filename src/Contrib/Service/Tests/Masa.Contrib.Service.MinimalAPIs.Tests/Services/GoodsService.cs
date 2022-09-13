// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class GoodsService : CustomServiceBase
{
    public GoodsService(IServiceCollection services) : base(services)
    {
        Url.Prefix = "api";
        Url.Version = "v2";
    }
}
