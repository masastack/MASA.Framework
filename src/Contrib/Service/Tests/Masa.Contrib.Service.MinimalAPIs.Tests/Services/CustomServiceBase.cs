// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public abstract class CustomServiceBase : ServiceBase
{
    protected CustomServiceBase()
    {
    }

    protected CustomServiceBase(string baseUri) : base(baseUri)
    {
    }

    public string TestGetBaseUri(ServiceRouteOptions globalOptions) => base.GetBaseUri(globalOptions,
        PluralizationService.CreateService(System.Globalization.CultureInfo.CreateSpecificCulture("en")));
}
