// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public abstract class CustomServiceBase : ServiceBase
{
    protected CustomServiceBase(IServiceCollection services) : base(services)
    {
    }

    protected CustomServiceBase(IServiceCollection services, string baseUri) : base(services, baseUri)
    {
    }

    public string TestGetBaseUri() => base.GetBaseUri();

    [IncludeMapping("get")]
    public string HealthChecks() => "success";
}
