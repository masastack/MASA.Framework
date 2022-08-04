// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class CustomService : ServiceBase
{
    private int _times = 0;

    public CustomService(IServiceCollection services) : base(services)
    {
        _times++;
    }

    public int Test() => _times;
}
