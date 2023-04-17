// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.MinimalAPIs.Tests.Services;

public class PersonService : ServiceBase
{
    public PersonService()
    {
        RouteOptions.DisableAutoMapRoute = true;
        App.MapGet("list", GetListAsync);
    }

    private Task GetListAsync()
    {
        return Task.CompletedTask;
    }
}
